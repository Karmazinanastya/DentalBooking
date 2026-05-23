using Microsoft.EntityFrameworkCore;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Repositories;
using ClinicService.Infrastructure.Persistence;

namespace ClinicService.Infrastructure.Repositories;

internal sealed class DoctorRepository(ClinicDbContext db) : IDoctorRepository
{
    public async Task<Doctor?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Doctors
            .Include(d => d.Services)
            .Include(d => d.ScheduleTemplates)
            .Include(d => d.ScheduleBlocks)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IReadOnlyList<Doctor>> GetByClinicAsync(Guid clinicId, CancellationToken ct = default) =>
        await db.Doctors
            .Include(d => d.Services)
            .Where(d => d.ClinicId == clinicId && d.IsActive)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Doctor>> GetByServiceAsync(Guid clinicId, Guid serviceId, CancellationToken ct = default) =>
        await db.Doctors
            .Include(d => d.Services)
            .Where(d => d.ClinicId == clinicId && d.IsActive && d.Services.Any(s => s.ServiceId == serviceId))
            .ToListAsync(ct);

    public async Task AddAsync(Doctor entity, CancellationToken ct = default) =>
        await db.Doctors.AddAsync(entity, ct);

    public void Update(Doctor entity) => db.Doctors.Update(entity);

    public void Remove(Doctor entity) => db.Doctors.Remove(entity);
}
