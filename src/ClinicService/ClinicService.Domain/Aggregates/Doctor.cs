using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Domain;
using ClinicService.Domain.DomainEvents;
using ClinicService.Domain.Entities;
using ClinicService.Domain.ValueObjects;

namespace ClinicService.Domain.Aggregates;

public sealed class Doctor : AggregateRoot<Guid>
{
    private readonly List<DoctorService> _services = [];
    private readonly List<ScheduleTemplate> _scheduleTemplates = [];
    private readonly List<ScheduleBlock> _scheduleBlocks = [];

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Specialization { get; private set; } = string.Empty;
    public string? PhotoUrl { get; private set; }
    public string? Bio { get; private set; }
    public bool IsActive { get; private set; }
    public Guid ClinicId { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public IReadOnlyList<DoctorService> Services => _services.AsReadOnly();
    public IReadOnlyList<ScheduleTemplate> ScheduleTemplates => _scheduleTemplates.AsReadOnly();
    public IReadOnlyList<ScheduleBlock> ScheduleBlocks => _scheduleBlocks.AsReadOnly();

    private Doctor() { }

    public static Doctor Create(
        Guid clinicId,
        string firstName,
        string lastName,
        string specialization,
        string? photoUrl = null,
        string? bio = null,
        Guid? seedId = null)
    {
        var doctor = new Doctor
        {
            Id = seedId ?? Guid.NewGuid(),
            ClinicId = clinicId,
            FirstName = firstName,
            LastName = lastName,
            Specialization = specialization,
            PhotoUrl = photoUrl,
            Bio = bio,
            IsActive = true
        };
        return doctor;
    }

    public void Update(string firstName, string lastName, string specialization, string? photoUrl, string? bio)
    {
        FirstName = firstName;
        LastName = lastName;
        Specialization = specialization;
        PhotoUrl = photoUrl;
        Bio = bio;
    }

    public Result AddService(Guid serviceId, int durationMinutes)
    {
        if (_services.Any(s => s.ServiceId == serviceId))
            return Result.Failure(Error.Conflict(nameof(DoctorService), "Service already assigned to this doctor."));

        _services.Add(DoctorService.Create(Id, serviceId, durationMinutes));
        return Result.Success();
    }

    public Result RemoveService(Guid serviceId)
    {
        var service = _services.FirstOrDefault(s => s.ServiceId == serviceId);
        if (service is null)
            return Result.Failure(Error.NotFound(nameof(DoctorService), serviceId));

        _services.Remove(service);
        return Result.Success();
    }

    public Result SetScheduleTemplate(
        DayOfWeek dayOfWeek,
        WorkingHours workingHours,
        WorkingHours? lunchBreak)
    {
        var existing = _scheduleTemplates.FirstOrDefault(t => t.DayOfWeek == dayOfWeek);
        if (existing is not null)
            existing.Update(workingHours, lunchBreak);
        else
            _scheduleTemplates.Add(ScheduleTemplate.Create(Id, ClinicId, dayOfWeek, workingHours, lunchBreak));

        RaiseDomainEvent(new DoctorScheduleUpdatedEvent(Guid.NewGuid(), DateTime.UtcNow, Id, ClinicId));
        return Result.Success();
    }

    public Result BlockDate(DateOnly date, string reason)
    {
        if (_scheduleBlocks.Any(b => b.Date == date))
            return Result.Failure(Error.Conflict(nameof(ScheduleBlock), $"Date {date} is already blocked."));

        _scheduleBlocks.Add(ScheduleBlock.Create(Id, ClinicId, date, reason));
        RaiseDomainEvent(new DoctorScheduleUpdatedEvent(Guid.NewGuid(), DateTime.UtcNow, Id, ClinicId));
        return Result.Success();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
