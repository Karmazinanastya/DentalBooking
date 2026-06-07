using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Slots.Queries.GetSlotInfo;

public sealed record GetSlotInfoQuery(Guid SlotId) : IRequest<Result<SlotInfoDto>>;
