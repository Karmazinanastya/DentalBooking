using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Domain;
using BookingService.Domain.DomainEvents;
using BookingService.Domain.Enums;

namespace BookingService.Domain.Aggregates;

public sealed class Appointment : AggregateRoot<Guid>
{
    private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan MinCancellationNotice = TimeSpan.FromHours(2);

    public Guid PatientId { get; private set; }
    public long PatientChatId { get; private set; }
    public Guid SlotId { get; private set; }
    public Guid DoctorId { get; private set; }
    public string DoctorFullName { get; private set; } = string.Empty;
    public Guid ClinicId { get; private set; }
    public string ClinicName { get; private set; } = string.Empty;
    public string ClinicAddress { get; private set; } = string.Empty;
    public string ClinicTimeZoneId { get; private set; } = string.Empty;
    public string ServiceName { get; private set; } = string.Empty;
    public DateTime AppointmentDateUtc { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Appointment() { }

    public static Appointment Create(
        Guid patientId,
        long patientChatId,
        Guid slotId,
        Guid doctorId,
        string doctorFullName,
        Guid clinicId,
        string clinicName,
        string clinicAddress,
        string clinicTimeZoneId,
        string serviceName,
        DateTime appointmentDateUtc)
    {
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            PatientChatId = patientChatId,
            SlotId = slotId,
            DoctorId = doctorId,
            DoctorFullName = doctorFullName,
            ClinicId = clinicId,
            ClinicName = clinicName,
            ClinicAddress = clinicAddress,
            ClinicTimeZoneId = clinicTimeZoneId,
            ServiceName = serviceName,
            AppointmentDateUtc = appointmentDateUtc,
            Status = AppointmentStatus.Pending,
            ExpiresAtUtc = DateTime.UtcNow.Add(HoldDuration),
            CreatedAtUtc = DateTime.UtcNow
        };

        return appointment;
    }

    public Result Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            return Result.Failure(Error.Conflict(nameof(Appointment), "Only pending appointments can be confirmed."));

        if (ExpiresAtUtc.HasValue && ExpiresAtUtc.Value < DateTime.UtcNow)
            return Result.Failure(Error.Conflict(nameof(Appointment), "Appointment hold has expired."));

        Status = AppointmentStatus.Confirmed;
        ExpiresAtUtc = null;

        RaiseDomainEvent(new AppointmentConfirmedDomainEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, PatientId, PatientChatId));

        RaiseDomainEvent(new AppointmentBookedDomainEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, PatientId, PatientChatId,
            DoctorId, DoctorFullName, ClinicId, ClinicName, ClinicAddress,
            ServiceName, AppointmentDateUtc, ClinicTimeZoneId));

        return Result.Success();
    }

    public Result CancelByPatient()
    {
        if (Status != AppointmentStatus.Confirmed && Status != AppointmentStatus.Pending)
            return Result.Failure(Error.Conflict(nameof(Appointment), "Appointment cannot be cancelled."));

        if (Status == AppointmentStatus.Confirmed &&
            AppointmentDateUtc - DateTime.UtcNow < MinCancellationNotice)
        {
            return Result.Failure(Error.Conflict(
                nameof(Appointment),
                $"Cannot cancel less than {MinCancellationNotice.TotalHours}h before appointment."));
        }

        Status = AppointmentStatus.CancelledByPatient;

        RaiseDomainEvent(new AppointmentCancelledByPatientDomainEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, PatientId, PatientChatId, AppointmentDateUtc));

        return Result.Success();
    }

    public Result CancelByClinic(string reason)
    {
        if (Status != AppointmentStatus.Confirmed)
            return Result.Failure(Error.Conflict(nameof(Appointment), "Only confirmed appointments can be cancelled by clinic."));

        Status = AppointmentStatus.CancelledByClinic;
        CancellationReason = reason;

        RaiseDomainEvent(new AppointmentCancelledByClinicDomainEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, PatientId, PatientChatId, AppointmentDateUtc, reason));

        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != AppointmentStatus.Confirmed)
            return Result.Failure(Error.Conflict(nameof(Appointment), "Only confirmed appointments can be completed."));

        Status = AppointmentStatus.Completed;

        RaiseDomainEvent(new AppointmentCompletedDomainEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, PatientId, PatientChatId, DoctorId, ClinicId));

        return Result.Success();
    }

    public Result Expire()
    {
        if (Status != AppointmentStatus.Pending)
            return Result.Failure(Error.Conflict(nameof(Appointment), "Only pending appointments can expire."));

        Status = AppointmentStatus.Expired;

        RaiseDomainEvent(new AppointmentExpiredDomainEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, SlotId));

        return Result.Success();
    }
}
