FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files
COPY ["src/Miru.Api/Miru.Api.csproj", "src/Miru.Api/"]
COPY ["src/Miru.Application/Miru.Application.csproj", "src/Miru.Application/"]
COPY ["src/Miru.Shared/Miru.Shared.csproj", "src/Miru.Shared/"]
COPY ["src/Miru.Domain/Miru.Domain.csproj", "src/Miru.Domain/"]
COPY ["src/Miru.Infrastructure/Miru.Infrastructure.csproj", "src/Miru.Infrastructure/"]

# Restore
RUN dotnet restore "src/Miru.Api/Miru.Api.csproj"

# Copy everything else
COPY . .

# Build
RUN dotnet build "src/Miru.Api/Miru.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/Miru.Api/Miru.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Miru.Api.dll"]
