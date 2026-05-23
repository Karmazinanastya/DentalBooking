using MediatR;
using Shared.BuildingBlocks.Common;

namespace BookingService.Application.Appointments.Commands.ExpireAppointments;

public sealed record ExpireAppointmentsCommand : IRequest<Result<int>>;
