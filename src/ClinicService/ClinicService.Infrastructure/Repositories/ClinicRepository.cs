using Microsoft.EntityFrameworkCore;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Repositories;
using ClinicService.Infrastructure.Persistence;

namespace ClinicService.Infrastructure.Repositories;

internal sealed class ClinicRepository(ClinicDbContext db) : IClinicRepository
{
    public async Task<Clinic?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Clinics.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Clinic>> GetByCityAsync(string city, CancellationToken ct = default) =>
        await db.Clinics
            .Where(c => c.IsActive && c.Address.City == city)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Clinic>> GetAllActiveAsync(CancellationToken ct = default) =>
        await db.Clinics.Where(c => c.IsActive).ToListAsync(ct);

    public async Task AddAsync(Clinic entity, CancellationToken ct = default) =>
        await db.Clinics.AddAsync(entity, ct);

    public void Update(Clinic entity) => db.Clinics.Update(entity);

    public void Remove(Clinic entity) => db.Clinics.Remove(entity);
}
