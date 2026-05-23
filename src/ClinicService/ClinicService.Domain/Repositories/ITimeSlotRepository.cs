using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Entities;
using ClinicService.Domain.Enums;

namespace ClinicService.Domain.Repositories;

public interface ITimeSlotRepository : IRepository<TimeSlot, Guid>
{
    Task AddRangeAsync(IReadOnlyList<TimeSlot> entities, CancellationToken ct = default);

    Task<IReadOnlyList<TimeSlot>> GetAvailableAsync(
        Guid doctorId,
        DateOnly date,
        CancellationToken ct = default);

    Task<IReadOnlyList<TimeSlot>> GetByDoctorAndRangeAsync(
        Guid doctorId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken ct = default);

    Task<bool> HasOverlapAsync(
        Guid doctorId,
        DateTime startUtc,
        DateTime endUtc,
        SlotStatus[] excludeStatuses,
        CancellationToken ct = default);

    Task ReleaseExpiredReservationsAsync(CancellationToken ct = default);
}
