using Shared.BuildingBlocks.Common;

namespace BookingService.Application.Abstractions;

public sealed record SlotInfo(
    Guid SlotId,
    Guid DoctorId,
    string DoctorFullName,
    Guid ClinicId,
    string ClinicName,
    string ClinicAddress,
    string ClinicTimeZoneId,
    string ServiceName,
    DateTime StartUtc
);

public interface ISlotService
{
    Task<Result<SlotInfo>> GetSlotInfoAsync(Guid slotId, CancellationToken ct = default);
    Task<Result> ReserveSlotAsync(Guid slotId, CancellationToken ct = default);
    Task<Result> BookSlotAsync(Guid slotId, CancellationToken ct = default);
    Task<Result> ReleaseSlotAsync(Guid slotId, CancellationToken ct = default);
}
