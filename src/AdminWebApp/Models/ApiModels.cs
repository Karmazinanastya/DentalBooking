namespace AdminWebApp.Models;

public sealed record LoginRequest(string Email, string Password);

public sealed record LoginResponse(
    string AccessToken,
    Guid UserId,
    string Email,
    string FullName,
    string Role,
    Guid? DoctorId,
    Guid? ClinicId);

public sealed record ClinicDto(
    Guid Id,
    string Name,
    string City,
    string Street,
    string BuildingNumber,
    string Phone,
    string? Description,
    string? PhotoUrl,
    string TimeZoneId);

public sealed record DoctorDto(Guid Id, string FullName, string Specialization);

public sealed record ServiceDto(
    Guid Id,
    string Name,
    string Category,
    string? Description,
    int DurationMinutes,
    decimal Price,
    bool IsActive);

public sealed record AppointmentDto(
    Guid Id,
    string DoctorFullName,
    string ClinicName,
    string ClinicAddress,
    string ServiceName,
    DateTime AppointmentDateUtc,
    string LocalDateTime,
    string Status,
    DateTime CreatedAtUtc);

public sealed record UserDto(
    Guid Id,
    string Email,
    string FullName,
    Guid? DoctorId,
    Guid? ClinicId,
    IReadOnlyList<string> Roles);

public sealed record CreateClinicRequest(
    string Name,
    string City,
    string Street,
    string BuildingNumber,
    string Phone,
    string TimeZoneId,
    string? Description);

public sealed record CreateDoctorRequest(
    Guid ClinicId,
    string FirstName,
    string LastName,
    string Specialization,
    string? Bio);

public sealed record CreateServiceRequest(
    string Name,
    string Category,
    int DurationMinutes,
    decimal Price,
    string? Description);

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string? LastName,
    string Role,
    Guid? DoctorId,
    Guid? ClinicId);
