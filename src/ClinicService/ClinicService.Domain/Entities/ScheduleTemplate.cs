using Shared.BuildingBlocks.Domain;
using ClinicService.Domain.ValueObjects;

namespace ClinicService.Domain.Entities;

public sealed class ScheduleTemplate : Entity<Guid>
{
    public Guid DoctorId { get; private set; }
    public Guid ClinicId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public WorkingHours WorkingHours { get; private set; } = null!;
    public WorkingHours? LunchBreak { get; private set; }
    public bool IsActive { get; private set; }

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
            WorkingHours = workingHours,
            LunchBreak = lunchBreak,
            IsActive = true
        };
    }

    public void Update(WorkingHours workingHours, WorkingHours? lunchBreak)
    {
        WorkingHours = workingHours;
        LunchBreak = lunchBreak;
    }

    public void Deactivate() => IsActive = false;
}
