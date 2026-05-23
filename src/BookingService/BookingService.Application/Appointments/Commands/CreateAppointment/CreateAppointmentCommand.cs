using MediatR;
using Shared.BuildingBlocks.Common;

namespace BookingService.Application.Appointments.Commands.CreateAppointment;

public sealed record CreateAppointmentCommand(
    Guid PatientId,
    long PatientChatId,
    Guid SlotId
) : IRequest<Result<Guid>>;
