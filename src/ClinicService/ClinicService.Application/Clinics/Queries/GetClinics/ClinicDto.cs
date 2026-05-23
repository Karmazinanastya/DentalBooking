namespace ClinicService.Application.Clinics.Queries.GetClinics;

public sealed record ClinicDto(
    Guid Id,
    string Name,
    string City,
    string Street,
    string BuildingNumber,
    string Phone,
    string? Description,
    string? PhotoUrl,
    string TimeZoneId
);
