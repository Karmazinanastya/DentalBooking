using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Slots.Commands.GenerateSlots;

public sealed record GenerateSlotsCommand(
    Guid DoctorId,
    DateOnly FromDate,
    DateOnly ToDate
) : IRequest<Result<int>>;
