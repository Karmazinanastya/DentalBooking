using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using BookingService.Application.Abstractions;
using BookingService.Domain.Aggregates;
using BookingService.Domain.Repositories;

namespace BookingService.Application.Appointments.Commands.CreateAppointment;

internal sealed class CreateAppointmentCommandHandler(
    IAppointmentRepository appointmentRepository,
    ISlotService slotService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        var slotInfoResult = await slotService.GetSlotInfoAsync(request.SlotId, ct);
        if (slotInfoResult.IsFailure)
            return Result.Failure<Guid>(slotInfoResult.Error);

        var slot = slotInfoResult.Value;

        var hasConflict = await appointmentRepository.HasActiveAppointmentAtTimeAsync(
            request.PatientId, slot.StartUtc, null, ct);

        if (hasConflict)
            return Result.Failure<Guid>(Error.Conflict(
                nameof(Appointment), "Patient already has an appointment at this time."));

        var reserveResult = await slotService.ReserveSlotAsync(request.SlotId, ct);
        if (reserveResult.IsFailure)
            return Result.Failure<Guid>(reserveResult.Error);

        var appointment = Appointment.Create(
            request.PatientId,
            request.PatientChatId,
            request.SlotId,
            slot.DoctorId,
            slot.DoctorFullName,
            slot.ClinicId,
            slot.ClinicName,
            slot.ClinicAddress,
            slot.ClinicTimeZoneId,
            slot.ServiceName,
            slot.StartUtc);

        await appointmentRepository.AddAsync(appointment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return appointment.Id;
    }
}
