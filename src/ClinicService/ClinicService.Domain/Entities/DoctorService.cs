using Shared.BuildingBlocks.Domain;

namespace ClinicService.Domain.Entities;

public sealed class DoctorService : Entity<Guid>
{
    public Guid DoctorId { get; private set; }
    public Guid ServiceId { get; private set; }
    public Service Service { get; private set; } = null!;
    public int CustomDurationMinutes { get; private set; }

    private DoctorService() { }

    public static DoctorService Create(Guid doctorId, Guid serviceId, int customDurationMinutes)
    {
        return new DoctorService
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            ServiceId = serviceId,
            CustomDurationMinutes = customDurationMinutes
        };
    }

    public void UpdateDuration(int durationMinutes) =>
        CustomDurationMinutes = durationMinutes;
}
