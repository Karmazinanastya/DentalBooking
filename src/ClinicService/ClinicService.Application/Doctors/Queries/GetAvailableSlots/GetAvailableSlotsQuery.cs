using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Doctors.Queries.GetAvailableSlots;

public sealed record GetAvailableSlotsQuery(
    Guid DoctorId,
    DateOnly Date
) : IRequest<Result<IReadOnlyList<SlotDto>>>;
