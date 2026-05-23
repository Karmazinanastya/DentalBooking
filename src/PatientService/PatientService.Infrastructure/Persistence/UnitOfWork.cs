using Shared.BuildingBlocks.Persistence;

namespace PatientService.Infrastructure.Persistence;

internal sealed class UnitOfWork(PatientDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
