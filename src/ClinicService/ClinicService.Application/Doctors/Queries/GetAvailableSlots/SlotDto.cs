namespace ClinicService.Application.Doctors.Queries.GetAvailableSlots;

public sealed record SlotDto(
    Guid SlotId,
    DateTime StartUtc,
    DateTime EndUtc,
    string LocalTime
);
