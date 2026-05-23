using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Domain;
using ClinicService.Domain.Enums;

namespace ClinicService.Domain.Entities;

public sealed class TimeSlot : Entity<Guid>
{
    public Guid DoctorId { get; private set; }
    public Guid ClinicId { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public SlotStatus Status { get; private set; }
    public DateTime? ReservedUntilUtc { get; private set; }

    private TimeSlot() { }

    public static TimeSlot Create(Guid doctorId, Guid clinicId, DateTime startUtc, DateTime endUtc)
    {
        return new TimeSlot
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            ClinicId = clinicId,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Status = SlotStatus.Available
        };
    }

    public Result Reserve(TimeSpan holdDuration)
    {
        if (Status != SlotStatus.Available)
            return Result.Failure(Error.Conflict(nameof(TimeSlot), "Slot is not available."));

        Status = SlotStatus.Reserved;
        ReservedUntilUtc = DateTime.UtcNow.Add(holdDuration);
        return Result.Success();
    }

    public Result Book()
    {
        if (Status != SlotStatus.Reserved)
            return Result.Failure(Error.Conflict(nameof(TimeSlot), "Slot must be reserved before booking."));

        Status = SlotStatus.Booked;
        ReservedUntilUtc = null;
        return Result.Success();
    }

    public void Release()
    {
        Status = SlotStatus.Available;
        ReservedUntilUtc = null;
    }

    public void Block()
    {
        Status = SlotStatus.Blocked;
        ReservedUntilUtc = null;
    }

    public bool IsExpiredReservation() =>
        Status == SlotStatus.Reserved && ReservedUntilUtc.HasValue && ReservedUntilUtc.Value < DateTime.UtcNow;
}
