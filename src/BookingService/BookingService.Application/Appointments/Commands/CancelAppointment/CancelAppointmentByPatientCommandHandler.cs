using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using BookingService.Application.Abstractions;
using BookingService.Domain.Aggregates;
using BookingService.Domain.Repositories;

namespace BookingService.Application.Appointments.Commands.CancelAppointment;

internal sealed class CancelAppointmentByPatientCommandHandler(
    IAppointmentRepository appointmentRepository,
    ISlotService slotService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CancelAppointmentByPatientCommand, Result>
{
    public async Task<Result> Handle(CancelAppointmentByPatientCommand request, CancellationToken ct)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            return Result.Failure(Error.NotFound(nameof(Appointment), request.AppointmentId));

        if (appointment.PatientId != request.PatientId)
            return Result.Failure(Error.Forbidden());

        var cancelResult = appointment.CancelByPatient();
        if (cancelResult.IsFailure)
            return cancelResult;

        await slotService.ReleaseSlotAsync(appointment.SlotId, ct);

        appointmentRepository.Update(appointment);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
