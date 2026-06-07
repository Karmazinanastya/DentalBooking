using Microsoft.EntityFrameworkCore;
using ClinicService.Domain.Entities;
using ClinicService.Domain.Enums;
using ClinicService.Domain.Repositories;
using ClinicService.Infrastructure.Persistence;

namespace ClinicService.Infrastructure.Repositories;

internal sealed class TimeSlotRepository(ClinicDbContext db) : ITimeSlotRepository
{
    public async Task<TimeSlot?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.TimeSlots.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<TimeSlot>> GetAvailableAsync(
        Guid doctorId, DateOnly date, CancellationToken ct = default)
    {
        var from = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var to = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        return await db.TimeSlots
            .Where(s => s.DoctorId == doctorId
                     && s.Status == SlotStatus.Available
                     && s.StartUtc >= from
                     && s.StartUtc <= to)
            .OrderBy(s => s.StartUtc)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TimeSlot>> GetByDoctorAndRangeAsync(
        Guid doctorId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default) =>
        await db.TimeSlots
            .Where(s => s.DoctorId == doctorId && s.StartUtc >= fromUtc && s.EndUtc <= toUtc)
            .OrderBy(s => s.StartUtc)
            .ToListAsync(ct);

    public async Task<bool> HasOverlapAsync(
        Guid doctorId, DateTime startUtc, DateTime endUtc,
        SlotStatus[] excludeStatuses, CancellationToken ct = default) =>
        await db.TimeSlots.AnyAsync(s =>
            s.DoctorId == doctorId
            && !excludeStatuses.Contains(s.Status)
            && s.StartUtc < endUtc
            && s.EndUtc > startUtc, ct);

    public async Task ReleaseExpiredReservationsAsync(CancellationToken ct = default)
    {
        var expired = await db.TimeSlots
            .Where(s => s.Status == SlotStatus.Reserved && s.ReservedUntilUtc < DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var slot in expired)
            slot.Release();
    }

    public async Task DeleteAvailableInRangeAsync(Guid doctorId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        var slots = await db.TimeSlots
            .Where(s => s.DoctorId == doctorId
                     && s.Status == SlotStatus.Available
                     && s.StartUtc >= fromUtc
                     && s.StartUtc < toUtc)
            .ToListAsync(ct);
        db.TimeSlots.RemoveRange(slots);
    }

    public async Task AddAsync(TimeSlot entity, CancellationToken ct = default) =>
        await db.TimeSlots.AddAsync(entity, ct);

    public async Task AddRangeAsync(IReadOnlyList<TimeSlot> entities, CancellationToken ct = default) =>
        await db.TimeSlots.AddRangeAsync(entities, ct);

    public void Update(TimeSlot entity) => db.TimeSlots.Update(entity);

    public void Remove(TimeSlot entity) => db.TimeSlots.Remove(entity);
}
