using Shared.BuildingBlocks.Persistence;

namespace ClinicService.Infrastructure.Persistence;

internal sealed class UnitOfWork(ClinicDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
