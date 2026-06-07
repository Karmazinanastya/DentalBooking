using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Entities;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Slots.Commands.ReserveSlot;

internal sealed class ReserveSlotCommandHandler(
    ITimeSlotRepository timeSlotRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ReserveSlotCommand, Result>
{
    private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(10);

    public async Task<Result> Handle(ReserveSlotCommand request, CancellationToken ct)
    {
        var slot = await timeSlotRepository.GetByIdAsync(request.SlotId, ct);
        if (slot is null)
            return Result.Failure(Error.NotFound(nameof(TimeSlot), request.SlotId));

        var result = slot.Reserve(HoldDuration);
        if (result.IsFailure)
            return result;

        timeSlotRepository.Update(slot);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
