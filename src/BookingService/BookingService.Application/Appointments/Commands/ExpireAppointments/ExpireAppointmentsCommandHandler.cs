using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using BookingService.Application.Abstractions;
using BookingService.Domain.Repositories;

namespace BookingService.Application.Appointments.Commands.ExpireAppointments;

internal sealed class ExpireAppointmentsCommandHandler(
    IAppointmentRepository appointmentRepository,
    ISlotService slotService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ExpireAppointmentsCommand, Result<int>>
{
    public async Task<Result<int>> Handle(ExpireAppointmentsCommand request, CancellationToken ct)
    {
        var expired = await appointmentRepository.GetExpiredPendingAsync(ct);

        foreach (var appointment in expired)
        {
            appointment.Expire();
            await slotService.ReleaseSlotAsync(appointment.SlotId, ct);
            appointmentRepository.Update(appointment);
        }

        if (expired.Count > 0)
            await unitOfWork.SaveChangesAsync(ct);

        return expired.Count;
    }
}
