// ============================================================
// LifetimeServices.cs — DI Lifetimes Demonstration Services
// ============================================================
// Mendefinisikan interface dan implementasi dengan 3 lifetime:
//   1. Transient: Instance baru setiap kali di-inject
//   2. Scoped: Instance yang sama selama satu HTTP Request
//   3. Singleton: Instance yang sama di seluruh siklus aplikasi
// ============================================================

namespace TesBackendNet.Framework.Services;

public interface ITransientService
{
    Guid GetOperationId();
}

public interface IScopedService
{
    Guid GetOperationId();
}

public interface ISingletonService
{
    Guid GetOperationId();
}

public class LifetimeDemoService : ITransientService, IScopedService, ISingletonService
{
    private readonly Guid _operationId = Guid.NewGuid();

    public Guid GetOperationId() => _operationId;
}
