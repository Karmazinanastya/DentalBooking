using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Slots.Commands.ReserveSlot;

public sealed record ReserveSlotCommand(Guid SlotId) : IRequest<Result>;
