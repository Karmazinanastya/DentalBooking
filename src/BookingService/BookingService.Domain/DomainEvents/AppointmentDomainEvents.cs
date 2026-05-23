using Shared.BuildingBlocks.Domain;

namespace BookingService.Domain.DomainEvents;

public sealed record AppointmentBookedDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId,
    Guid DoctorId,
    string DoctorFullName,
    Guid ClinicId,
    string ClinicName,
    string ClinicAddress,
    string ServiceName,
    DateTime AppointmentDateUtc,
    string ClinicTimeZoneId
) : IDomainEvent;

public sealed record AppointmentConfirmedDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId
) : IDomainEvent;

public sealed record AppointmentCancelledByPatientDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId,
    DateTime AppointmentDateUtc
) : IDomainEvent;

public sealed record AppointmentCancelledByClinicDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId,
    DateTime AppointmentDateUtc,
    string Reason
) : IDomainEvent;

public sealed record AppointmentCompletedDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId,
    Guid DoctorId,
    Guid ClinicId
) : IDomainEvent;

public sealed record AppointmentExpiredDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid SlotId
) : IDomainEvent;
