using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using BookingService.Application.Abstractions;
using BookingService.Domain.Aggregates;
using BookingService.Domain.Repositories;

namespace BookingService.Application.Appointments.Commands.CancelAppointment;

internal sealed class CancelAppointmentByClinicCommandHandler(
    IAppointmentRepository appointmentRepository,
    ISlotService slotService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CancelAppointmentByClinicCommand, Result>
{
    public async Task<Result> Handle(CancelAppointmentByClinicCommand request, CancellationToken ct)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            return Result.Failure(Error.NotFound(nameof(Appointment), request.AppointmentId));

        if (appointment.ClinicId != request.ClinicId)
            return Result.Failure(Error.Forbidden());

        var cancelResult = appointment.CancelByClinic(request.Reason);
        if (cancelResult.IsFailure)
            return cancelResult;

        await slotService.ReleaseSlotAsync(appointment.SlotId, ct);

        appointmentRepository.Update(appointment);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
