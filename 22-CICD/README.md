# 🔄 22 — CI/CD

## Apa itu CI/CD?

**CI** (Continuous Integration): otomatis build dan test setiap kali ada push ke repository.

**CD** (Continuous Delivery/Deployment): otomatis deploy ke server setelah CI berhasil.

---

## GitHub Actions untuk .NET

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore --configuration Release

    - name: Run tests
      run: dotnet test --no-build --configuration Release --verbosity normal

    - name: Build Docker image
      if: github.ref == 'refs/heads/main'
      run: docker build -t myapp:${{ github.sha }} .

    - name: Deploy to server
      if: github.ref == 'refs/heads/main'
      run: |
        # SSH ke server dan update container
        ssh user@server "docker pull myapp:latest && docker-compose up -d"
```

---

## Pipeline Stages

```
Code Push
    ↓
Lint & Format Check
    ↓
Unit Tests
    ↓
Integration Tests
    ↓
Build Docker Image
    ↓
Push to Registry
    ↓
Deploy to Staging
    ↓
Smoke Tests
    ↓
Deploy to Production ← (Manual approval atau auto)
```

---

## Environment Variables di CI/CD

```yaml
# GitHub Secrets
steps:
  - name: Deploy
    env:
      DB_CONNECTION: ${{ secrets.DB_CONNECTION_STRING }}
      JWT_SECRET: ${{ secrets.JWT_SECRET_KEY }}
    run: |
      export ConnectionStrings__Default="$DB_CONNECTION"
      docker-compose up -d
```

---

## ✅ Best Practice

1. **Test sebelum merge**: wajib passing sebelum PR di-merge
2. **Environment terpisah**: dev → staging → production
3. **Secrets management**: jangan hardcode, gunakan GitHub Secrets/Vault
4. **Rollback plan**: bisa rollback ke versi sebelumnya
5. **Blue-Green deployment**: deploy ke environment baru, switch traffic

---

## 🎤 Tips Interview

**Q: "Apa bedanya CI dan CD?"**
```
CI: Continuous Integration
   → Otomatis build + test setiap push
   → Cepat detect masalah

CD: Continuous Delivery/Deployment
   → Delivery: siap deploy, butuh approval manual
   → Deployment: otomatis deploy ke production
```
