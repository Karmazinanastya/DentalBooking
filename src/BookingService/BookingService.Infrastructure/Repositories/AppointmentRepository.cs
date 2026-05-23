using Microsoft.EntityFrameworkCore;
using BookingService.Domain.Aggregates;
using BookingService.Domain.Enums;
using BookingService.Domain.Repositories;
using BookingService.Infrastructure.Persistence;

namespace BookingService.Infrastructure.Repositories;

internal sealed class AppointmentRepository(BookingDbContext db) : IAppointmentRepository
{
    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Appointments.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<Appointment>> GetByPatientAsync(Guid patientId, CancellationToken ct = default) =>
        await db.Appointments
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDateUtc)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Appointment>> GetByClinicAsync(
        Guid clinicId, AppointmentStatus? status, CancellationToken ct = default)
    {
        var query = db.Appointments.Where(a => a.ClinicId == clinicId);
        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);
        return await query.OrderBy(a => a.AppointmentDateUtc).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Appointment>> GetExpiredPendingAsync(CancellationToken ct = default) =>
        await db.Appointments
            .Where(a => a.Status == AppointmentStatus.Pending
                     && a.ExpiresAtUtc.HasValue
                     && a.ExpiresAtUtc.Value < DateTime.UtcNow)
            .ToListAsync(ct);

    public async Task<bool> HasActiveAppointmentAtTimeAsync(
        Guid patientId, DateTime dateTimeUtc, Guid? excludeId, CancellationToken ct = default)
    {
        var activeStatuses = new[] { AppointmentStatus.Pending, AppointmentStatus.Confirmed };
        return await db.Appointments.AnyAsync(a =>
            a.PatientId == patientId
            && activeStatuses.Contains(a.Status)
            && a.AppointmentDateUtc == dateTimeUtc
            && (excludeId == null || a.Id != excludeId.Value), ct);
    }

    public async Task AddAsync(Appointment entity, CancellationToken ct = default) =>
        await db.Appointments.AddAsync(entity, ct);

    public void Update(Appointment entity) => db.Appointments.Update(entity);

    public void Remove(Appointment entity) => db.Appointments.Remove(entity);
}
