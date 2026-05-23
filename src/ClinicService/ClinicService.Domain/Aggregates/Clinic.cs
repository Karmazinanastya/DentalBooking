using Shared.BuildingBlocks.Domain;
using ClinicService.Domain.ValueObjects;

namespace ClinicService.Domain.Aggregates;

public sealed class Clinic : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public Address Address { get; private set; } = null!;
    public string Phone { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? PhotoUrl { get; private set; }
    public string TimeZoneId { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Clinic() { }

    public static Clinic Create(
        string name,
        Address address,
        string phone,
        string timeZoneId,
        string? description = null,
        string? photoUrl = null)
    {
        return new Clinic
        {
            Id = Guid.NewGuid(),
            Name = name,
            Address = address,
            Phone = phone,
            TimeZoneId = timeZoneId,
            Description = description,
            PhotoUrl = photoUrl,
            IsActive = true
        };
    }

    public void Update(
        string name,
        Address address,
        string phone,
        string timeZoneId,
        string? description,
        string? photoUrl)
    {
        Name = name;
        Address = address;
        Phone = phone;
        TimeZoneId = timeZoneId;
        Description = description;
        PhotoUrl = photoUrl;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
