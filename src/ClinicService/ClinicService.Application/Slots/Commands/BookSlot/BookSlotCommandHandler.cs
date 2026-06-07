using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Entities;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Slots.Commands.BookSlot;

internal sealed class BookSlotCommandHandler(
    ITimeSlotRepository timeSlotRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<BookSlotCommand, Result>
{
    public async Task<Result> Handle(BookSlotCommand request, CancellationToken ct)
    {
        var slot = await timeSlotRepository.GetByIdAsync(request.SlotId, ct);
        if (slot is null)
            return Result.Failure(Error.NotFound(nameof(TimeSlot), request.SlotId));

        var result = slot.Book();
        if (result.IsFailure)
            return result;

        timeSlotRepository.Update(slot);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
