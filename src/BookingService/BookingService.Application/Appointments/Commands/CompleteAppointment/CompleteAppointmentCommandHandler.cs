using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using BookingService.Domain.Aggregates;
using BookingService.Domain.Repositories;

namespace BookingService.Application.Appointments.Commands.CompleteAppointment;

internal sealed class CompleteAppointmentCommandHandler(
    IAppointmentRepository appointmentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CompleteAppointmentCommand, Result>
{
    public async Task<Result> Handle(CompleteAppointmentCommand request, CancellationToken ct)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            return Result.Failure(Error.NotFound(nameof(Appointment), request.AppointmentId));

        if (appointment.ClinicId != request.ClinicId)
            return Result.Failure(Error.Forbidden());

        var result = appointment.Complete();
        if (result.IsFailure)
            return result;

        appointmentRepository.Update(appointment);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
