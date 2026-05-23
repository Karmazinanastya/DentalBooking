namespace PatientService.Application.Patients.Queries.GetPatient;

public sealed record PatientDto(
    Guid Id,
    long ChatId,
    string FirstName,
    string LastName,
    string FullName,
    string PhoneNumber,
    DateTime RegisteredAtUtc
);
