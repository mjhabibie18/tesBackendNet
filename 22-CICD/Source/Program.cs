// ============================================================
// Program.cs — Simulasi Eksekusi CI/CD Pipeline Runner
// ============================================================
// Proyek ini mendemonstrasikan secara visual langkah-langkah
// pipeline otomatisasi Continuous Integration dan Continuous Deployment
// untuk project ASP.NET Core (.NET 8/9).
// ============================================================

using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================================");
        Console.WriteLine("🚀 AUTOMATED CI/CD PIPELINE RUNNER SIMULATOR (.NET 8/9)");
        Console.WriteLine("============================================================");
        Console.ResetColor();

        Console.WriteLine("\n[Trigger] GitHub Webhook: Push event detected on branch 'main'.");
        Console.WriteLine("Memulai Pipeline Otomatis...\n");

        // ── STAGE 1: CONTINUOUS INTEGRATION (CI) ─────────────────
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(">>> STAGE 1: CONTINUOUS INTEGRATION (CI)");
        Console.ResetColor();

        // Step 1.1: NuGet Restore
        RunPipelineStep("Step 1.1: NuGet Restore", () =>
        {
            Console.WriteLine("  Determining projects to restore...");
            Console.WriteLine("  Restoring packages for d:\\Code\\tesBackendNet\\01-CRUD\\Source\\TesBackendNet.csproj...");
            Console.WriteLine("  Restoring packages for d:\\Code\\tesBackendNet\\18-Testing\\Tests\\TesBackendNet.Tests.csproj...");
            Thread.Sleep(1200);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [SUCCESS] All packages restored successfully.");
            Console.ResetColor();
        });

        // Step 1.2: Dotnet Build
        RunPipelineStep("Step 1.2: Build Compilation", () =>
        {
            Console.WriteLine("  Microsoft (R) Build Engine version 17.8.3 for .NET");
            Console.WriteLine("  Compiling Source C# files...");
            Thread.Sleep(1500);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [SUCCESS] TesBackendNet.csproj -> d:\\bin\\Release\\net8.0\\app.dll");
            Console.WriteLine("  [SUCCESS] 0 Warning(s), 0 Error(s)");
            Console.ResetColor();
        });

        // Step 1.3: Run Unit Tests
        RunPipelineStep("Step 1.3: Automated Unit Testing", () =>
        {
            Console.WriteLine("  Starting test execution, please wait...");
            Console.WriteLine("  [xUnit.net 2.5.3] running tests in TesBackendNet.Tests.dll");
            Thread.Sleep(1000);
            Console.WriteLine("  ✔ TesBackendNet.Tests.ProductServiceTests.CreateProduct_Should_Succeed [PASS]");
            Console.WriteLine("  ✔ TesBackendNet.Tests.ProductServiceTests.GetProduct_With_NonExistentId_Should_Return_Null [PASS]");
            Console.WriteLine("  ✔ TesBackendNet.Tests.AuthServiceTests.Login_With_ValidCredentials_Should_Return_Token [PASS]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [SUCCESS] Passed: 3, Failed: 0, Total: 3 (Duration: 0.85s)");
            Console.ResetColor();
        });

        // ── STAGE 2: CONTINUOUS DEPLOYMENT (CD) ─────────────────
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n>>> STAGE 2: CONTINUOUS DEPLOYMENT (CD)");
        Console.ResetColor();

        // Step 2.1: Docker Containerization
        RunPipelineStep("Step 2.1: Docker Build & Package", () =>
        {
            Console.WriteLine("  Sending build context to Docker daemon...");
            Console.WriteLine("  Step 1/8 : FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base");
            Console.WriteLine("  Step 2/8 : WORKDIR /app");
            Console.WriteLine("  Step 3/8 : COPY publish/ .");
            Console.WriteLine("  Step 4/8 : ENTRYPOINT [\"dotnet\", \"app.dll\"]");
            Thread.Sleep(1800);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [SUCCESS] Successfully built image 'mjhabibie18/tesbackendnet:latest'.");
            Console.ResetColor();
        });

        // Step 2.2: Push to Registry
        RunPipelineStep("Step 2.2: Push Image to GitHub Container Registry (GHCR)", () =>
        {
            Console.WriteLine("  The push refers to repository [ghcr.io/mjhabibie18/tesbackendnet]");
            Console.WriteLine("  Preparing layer [a1b2c3d4e5f6]...");
            Console.WriteLine("  Pushing layer [a1b2c3d4e5f6]...");
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [SUCCESS] ghcr.io/mjhabibie18/tesbackendnet:latest - Pushed");
            Console.ResetColor();
        });

        // Step 2.3: Kubernetes/AppService Rolling Update
        RunPipelineStep("Step 2.3: Production Deployment (Rolling Update)", () =>
        {
            Console.WriteLine("  Connecting to Kubernetes cluster api.k8s.production...");
            Console.WriteLine("  Updating deployment 'tesbackendnet-api' to use image 'latest'...");
            Console.WriteLine("  Waiting for rollout to finish (1/2 replicas updated)...");
            Thread.Sleep(1200);
            Console.WriteLine("  Deployment rollout completed successfully.");
            
            // Healthcheck ping simulation
            Console.WriteLine("  Performing Smoke-test ping to production health check: /health...");
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [SUCCESS] HTTP status 200 OK received. Healthcheck PASSED.");
            Console.ResetColor();
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n============================================================");
        Console.WriteLine("🎉 ALL PIPELINE JOBS COMPLETED SUCCESSFULLY!");
        Console.WriteLine("Status: Production deployment is active and healthy.");
        Console.WriteLine("============================================================");
        Console.ResetColor();
    }

    private static void RunPipelineStep(string stepName, Action action)
    {
        Console.Write($"Running: {stepName}...");
        Thread.Sleep(500);
        Console.WriteLine();
        action();
        Console.WriteLine(new string('-', 50));
    }
}
