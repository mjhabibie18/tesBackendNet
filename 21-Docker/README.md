# 🐳 21 — Docker

## Apa itu Docker?

Docker adalah platform containerization yang memungkinkan aplikasi berjalan dalam lingkungan yang terisolasi (container).

```
Tanpa Docker:
"Di komputerku berjalan, di server tidak!" 😢

Dengan Docker:
"Build once, run anywhere!" 🚀
```

---

## Konsep Dasar

| Konsep | Deskripsi |
|--------|-----------|
| **Image** | Blueprint/template aplikasi (read-only) |
| **Container** | Instance dari image (running) |
| **Dockerfile** | Instruksi untuk build image |
| **Docker Compose** | Orchestrate multi-container apps |
| **Registry** | Penyimpanan image (Docker Hub, ACR) |

---

## Dockerfile untuk ASP.NET Core

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file dan restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy semua file dan build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime (lebih kecil dari SDK image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy dari stage build
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

---

## Docker Compose untuk Development

```yaml
version: '3.9'

services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ConnectionStrings__Default=Server=sqlserver;Database=MyDb;...
      - Jwt__SecretKey=your-secret-key
    depends_on:
      - sqlserver
    volumes:
      - ./logs:/app/logs

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "StrongPassword@2024"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
```

---

## Perintah Docker Penting

```bash
# Build image
docker build -t myapp:latest .

# Run container
docker run -d -p 8080:8080 --name myapp myapp:latest

# Docker Compose
docker-compose up -d         # Start semua service
docker-compose down          # Stop semua service
docker-compose logs -f api   # Follow logs service 'api'
docker-compose ps            # Status container

# Debug
docker exec -it myapp bash   # Masuk ke container
docker logs myapp -f         # View logs
docker stats                 # Resource usage
```

---

## 🎤 Tips Interview

**Q: "Apa bedanya Docker image dan container?"**
```
Image: template/blueprint (seperti class di OOP)
Container: instance yang running (seperti object di OOP)
Satu image bisa dibuat banyak container
```

**Q: "Apa itu multi-stage build?"**
```
Build dalam beberapa stage untuk menghasilkan image yang lebih kecil.
Stage 1: SDK image untuk build (besar)
Stage 2: Runtime image untuk produksi (kecil)
Hasil: image produksi tanpa SDK → lebih kecil dan aman
```
