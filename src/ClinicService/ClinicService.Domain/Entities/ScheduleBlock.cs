using Shared.BuildingBlocks.Domain;

namespace ClinicService.Domain.Entities;

public sealed class ScheduleBlock : Entity<Guid>
{
    public Guid DoctorId { get; private set; }
    public Guid ClinicId { get; private set; }
    public DateOnly Date { get; private set; }
    public string Reason { get; private set; } = string.Empty;

    private ScheduleBlock() { }

    public static ScheduleBlock Create(Guid doctorId, Guid clinicId, DateOnly date, string reason)
    {
        return new ScheduleBlock
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            ClinicId = clinicId,
            Date = date,
            Reason = reason
        };
    }
}
