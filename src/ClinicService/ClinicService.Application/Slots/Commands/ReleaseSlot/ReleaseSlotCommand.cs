using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Slots.Commands.ReleaseSlot;

public sealed record ReleaseSlotCommand(Guid SlotId) : IRequest<Result>;
