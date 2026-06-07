namespace ClinicService.Application.Slots.Queries.GetSlotInfo;

public sealed record SlotInfoDto(
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
