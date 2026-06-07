using Microsoft.EntityFrameworkCore;
using ClinicService.Domain.Entities;
using ClinicService.Domain.Repositories;
using ClinicService.Infrastructure.Persistence;

namespace ClinicService.Infrastructure.Repositories;

internal sealed class ServiceRepository(ClinicDbContext db) : IServiceRepository
{
    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Services.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Service>> GetAllActiveAsync(CancellationToken ct = default) =>
        await db.Services.Where(s => s.IsActive).OrderBy(s => s.Category).ThenBy(s => s.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<Service>> GetByCategoryAsync(string category, CancellationToken ct = default) =>
        await db.Services.Where(s => s.IsActive && s.Category == category).ToListAsync(ct);

    public async Task<IReadOnlyList<Service>> GetByClinicAsync(Guid clinicId, CancellationToken ct = default) =>
        await db.Services.Where(s => s.IsActive).ToListAsync(ct);

    public async Task AddAsync(Service entity, CancellationToken ct = default) =>
        await db.Services.AddAsync(entity, ct);

    public void Update(Service entity) => db.Services.Update(entity);

    public void Remove(Service entity) => db.Services.Remove(entity);
}
