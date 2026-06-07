using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Entities;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Slots.Commands.ReleaseSlot;

internal sealed class ReleaseSlotCommandHandler(
    ITimeSlotRepository timeSlotRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ReleaseSlotCommand, Result>
{
    public async Task<Result> Handle(ReleaseSlotCommand request, CancellationToken ct)
    {
        var slot = await timeSlotRepository.GetByIdAsync(request.SlotId, ct);
        if (slot is null)
            return Result.Failure(Error.NotFound(nameof(TimeSlot), request.SlotId));

        slot.Release();
        timeSlotRepository.Update(slot);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
