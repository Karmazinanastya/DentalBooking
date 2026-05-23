using Shared.BuildingBlocks.Persistence;

namespace BookingService.Infrastructure.Persistence;

internal sealed class UnitOfWork(BookingDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
