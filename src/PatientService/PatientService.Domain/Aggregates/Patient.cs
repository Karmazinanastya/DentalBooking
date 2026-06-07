using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Domain;
using PatientService.Domain.DomainEvents;

namespace PatientService.Domain.Aggregates;

public sealed class Patient : AggregateRoot<Guid>
{
    public long ChatId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public DateTime RegisteredAtUtc { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private Patient() { }

    public static Result<Patient> Create(
        long chatId,
        string firstName,
        string lastName,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<Patient>(Error.Validation(nameof(FirstName), "First name is required."));


        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure<Patient>(Error.Validation(nameof(PhoneNumber), "Phone number is required."));

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            RegisteredAtUtc = DateTime.UtcNow
        };

        patient.RaiseDomainEvent(new PatientRegisteredDomainEvent(
            Guid.NewGuid(), DateTime.UtcNow, patient.Id, chatId, patient.FullName));

        return patient;
    }

    public void Update(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }
}
