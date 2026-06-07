using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Slots.Commands.BookSlot;

public sealed record BookSlotCommand(Guid SlotId) : IRequest<Result>;
