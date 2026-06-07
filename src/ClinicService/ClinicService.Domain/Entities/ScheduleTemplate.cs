using Shared.BuildingBlocks.Domain;
using ClinicService.Domain.ValueObjects;

namespace ClinicService.Domain.Entities;

public sealed class ScheduleTemplate : Entity<Guid>
{
    public Guid DoctorId { get; private set; }
    public Guid ClinicId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly WorkStart { get; private set; }
    public TimeOnly WorkEnd { get; private set; }
    public TimeOnly? LunchStart { get; private set; }
    public TimeOnly? LunchEnd { get; private set; }
    public bool IsActive { get; private set; }

    public WorkingHours WorkingHours => WorkingHours.Create(WorkStart, WorkEnd).Value;
    public WorkingHours? LunchBreak => LunchStart.HasValue && LunchEnd.HasValue
        ? WorkingHours.Create(LunchStart.Value, LunchEnd.Value).Value
        : null;

    private ScheduleTemplate() { }

    public static ScheduleTemplate Create(
        Guid doctorId,
        Guid clinicId,
        DayOfWeek dayOfWeek,
        WorkingHours workingHours,
        WorkingHours? lunchBreak = null)
    {
        return new ScheduleTemplate
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            ClinicId = clinicId,
            DayOfWeek = dayOfWeek,
            WorkStart = workingHours.Start,
            WorkEnd = workingHours.End,
            LunchStart = lunchBreak?.Start,
            LunchEnd = lunchBreak?.End,
            IsActive = true
        };
    }

    public void Update(WorkingHours workingHours, WorkingHours? lunchBreak)
    {
        WorkStart = workingHours.Start;
        WorkEnd = workingHours.End;
        LunchStart = lunchBreak?.Start;
        LunchEnd = lunchBreak?.End;
    }

    public void Deactivate() => IsActive = false;
}
