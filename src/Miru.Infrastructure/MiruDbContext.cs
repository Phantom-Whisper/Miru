using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Miru.Domain.Entities;

namespace Miru.Infrastructure
{
    public class MiruDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Serie> Series => Set<Serie>();
        public DbSet<Season> Seasons => Set<Season>();
        public DbSet<Episode> Episodes => Set<Episode>();

        public MiruDbContext() 
        { }

        public MiruDbContext(DbContextOptions<MiruDbContext> options) 
            : base(options) 
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply configurations from /Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiruDbContext).Assembly);
        }
    }
}
