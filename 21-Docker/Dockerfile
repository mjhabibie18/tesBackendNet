# ============================================================
# Dockerfile — Multi-Stage Build ASP.NET Core API
# ============================================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files dan restore package dependencies
COPY ["Source/TesBackendNet.DockerDemo.csproj", "Source/"]
RUN dotnet restore "Source/TesBackendNet.DockerDemo.csproj"

# Copy seluruh source code
COPY . .
WORKDIR "/src/Source"
RUN dotnet publish "TesBackendNet.DockerDemo.csproj" -c Release -o /app/publish

# Stage 2: Runtime (Image produksi berukuran kecil)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port HTTP default container
EXPOSE 8080

# Konfigurasi environment variable untuk ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TesBackendNet.DockerDemo.dll"]
