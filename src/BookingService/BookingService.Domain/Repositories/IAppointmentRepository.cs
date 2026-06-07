using Shared.BuildingBlocks.Persistence;
using BookingService.Domain.Aggregates;
using BookingService.Domain.Enums;

namespace BookingService.Domain.Repositories;

public interface IAppointmentRepository : IRepository<Appointment, Guid>
{
    Task<IReadOnlyList<Appointment>> GetByPatientAsync(Guid patientId, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetByClinicAsync(Guid clinicId, AppointmentStatus? status, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetAllAsync(Guid? clinicId, DateOnly? date, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetExpiredPendingAsync(CancellationToken ct = default);
    Task<bool> HasActiveAppointmentAtTimeAsync(Guid patientId, DateTime dateTimeUtc, Guid? excludeId, CancellationToken ct = default);
}
