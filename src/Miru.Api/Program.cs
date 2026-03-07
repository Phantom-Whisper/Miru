using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Miru.Application.Mappings;
using Miru.Application.Services;
using Miru.Contracts.Persistence;
using Miru.Contracts.Services;
using Miru.Domain.Entities;
using Miru.Infrastructure;
using Miru.Infrastructure.Persistence;
using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MiruDb");

builder.Services.AddDbContext<MiruDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthorization();

builder.Services
    .AddIdentityApiEndpoints<UserEntity>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<MiruDbContext>();

// AutoMapper
builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApiDocument(options =>
{
    options.PostProcess = document =>
    {
        document.Info = new OpenApiInfo
        {
            Version = "v1",
            Title = "Miru API",
            Description = "Miru API",
            TermsOfService = "None",
            /*Contact = new NSwag.OpenApiContact
            {
                Name = "Miru",
                Url = "https://github.com/miru/miru-api"
            },
            License = new NSwag.OpenApiLicense
            {
                Name = "Miru",
                Url = "https://github.com/miru/miru-api"
            }*/
        };
        // document.Servers.Add(new OpenApiServer() { Url =  $"https://miru-api.miru-api.com/api/v1/" });
    };
    options.AddSecurity("JWT", [], new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Bearer {token}."
    });
    options.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT"));

});

builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ISerieService, SerieService>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<IEpisodeService, EpisodeService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole",
        policy => policy.RequireRole("ADMIN"));
});


var app = builder.Build();

// app.UseMiddleware<RequestLoggingMiddleware>(); // TODO: Need to create a class

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

    // Roles
    var roles = new[] { "ADMIN", "USER" };
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    // Admin user
    var adminEmail = builder.Configuration["AdminUser:Email"];
    var adminPassword = builder.Configuration["AdminUser:Password"];

    if (await userManager.FindByEmailAsync(adminEmail!) == null)
    {
        var admin = new UserEntity
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword!);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "ADMIN");
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create admin user: {errors}");
        }
    }
}

app.MapGroup("authentication")
    .MapIdentityApi<UserEntity>()
    .WithTags("Auth");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
