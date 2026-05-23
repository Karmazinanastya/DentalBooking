using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using BookingService.Application.Abstractions;
using BookingService.Domain.Aggregates;
using BookingService.Domain.Repositories;

namespace BookingService.Application.Appointments.Commands.ConfirmAppointment;

internal sealed class ConfirmAppointmentCommandHandler(
    IAppointmentRepository appointmentRepository,
    ISlotService slotService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ConfirmAppointmentCommand, Result>
{
    public async Task<Result> Handle(ConfirmAppointmentCommand request, CancellationToken ct)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            return Result.Failure(Error.NotFound(nameof(Appointment), request.AppointmentId));

        if (appointment.PatientId != request.PatientId)
            return Result.Failure(Error.Forbidden());

        var confirmResult = appointment.Confirm();
        if (confirmResult.IsFailure)
            return confirmResult;

        var bookSlotResult = await slotService.BookSlotAsync(appointment.SlotId, ct);
        if (bookSlotResult.IsFailure)
            return bookSlotResult;

        appointmentRepository.Update(appointment);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
