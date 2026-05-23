namespace TelegramBotService.HttpClients;

public sealed record PatientResponse(Guid Id, long ChatId, string FullName, string PhoneNumber);

public sealed record ClinicResponse(Guid Id, string Name, string City, string Street, string BuildingNumber, string TimeZoneId);

public sealed record DoctorResponse(Guid Id, string FullName, string Specialization);

public sealed record SlotResponse(Guid SlotId, string LocalTime, DateTime StartUtc);

public sealed record AppointmentResponse(
    Guid Id,
    string DoctorFullName,
    string ClinicName,
    string ClinicAddress,
    string ServiceName,
    string LocalDateTime,
    string Status);

public sealed record CreateAppointmentRequest(Guid PatientId, long PatientChatId, Guid SlotId);

public sealed record RegisterPatientRequest(long ChatId, string FirstName, string LastName, string PhoneNumber);
