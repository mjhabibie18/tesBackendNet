# ☁️ 29 — Cloud

## Cloud untuk Backend Developer

Cloud provider populer: **Azure** (Microsoft), **AWS** (Amazon), **GCP** (Google).

---

## Azure Services untuk .NET Developer

| Service | Fungsi | Ekuivalen Lokal |
|---------|--------|----------------|
| **Azure App Service** | Host web API | IIS / Kestrel |
| **Azure SQL Database** | Managed SQL Server | SQL Server lokal |
| **Azure Blob Storage** | File storage | Local file system |
| **Azure Key Vault** | Secrets management | appsettings.json |
| **Azure Service Bus** | Message queue | RabbitMQ |
| **Azure Redis Cache** | Distributed cache | Redis lokal |
| **Azure Container Apps** | Container hosting | Docker |

---

## Deploy ke Azure App Service

```bash
# Install Azure CLI
az login

# Buat resource group
az group create --name myRG --location "Southeast Asia"

# Buat App Service Plan
az appservice plan create --name myPlan --resource-group myRG --sku B1

# Buat Web App
az webapp create --name myapp-api --resource-group myRG --plan myPlan --runtime "dotnet:8"

# Deploy
dotnet publish -c Release -o ./publish
az webapp deployment source config-zip \
  --resource-group myRG \
  --name myapp-api \
  --src publish.zip
```

---

## Environment Variables di Azure

```bash
# Set app settings (= environment variables di Azure)
az webapp config appsettings set \
  --name myapp-api \
  --resource-group myRG \
  --settings \
    "ConnectionStrings__Default=Server=..." \
    "Jwt__SecretKey=your-secret"
```

---

## Azure Key Vault

```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

```csharp
// Baca secrets dari Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://your-vault.vault.azure.net/"),
    new DefaultAzureCredential());
```

---

## Azure Blob Storage (File Upload)

```bash
dotnet add package Azure.Storage.Blobs
```

```csharp
var container = new BlobContainerClient(connectionString, "uploads");
await container.CreateIfNotExistsAsync();

var blob = container.GetBlobClient($"{Guid.NewGuid()}.jpg");
await blob.UploadAsync(fileStream);

var url = blob.Uri.ToString(); // Public URL
```

---

## ✅ Best Practice

1. **Gunakan Managed Identity**: tidak perlu connection string di app
2. **Key Vault untuk secrets**: bukan environment variable
3. **CDN untuk static files**: lebih cepat dari blob storage langsung
4. **Auto-scaling**: scale out saat load tinggi
5. **Health checks + Application Insights**: monitoring production

---

## 🎤 Tips Interview

**Q: "Apa bedanya IaaS, PaaS, SaaS?"**
```
IaaS (Infrastructure as a Service): virtual machine, storage, network
     Contoh: Azure VM → kamu manage OS, runtime, app

PaaS (Platform as a Service): platform siap pakai
     Contoh: Azure App Service → kamu hanya deploy app

SaaS (Software as a Service): software siap pakai
     Contoh: Office 365, Salesforce
```
