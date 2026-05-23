using MediatR;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Application.Slots.Commands.GenerateSlots;
using ClinicService.Domain.DomainEvents;

namespace ClinicService.Application.DomainEventHandlers;

internal sealed class DoctorScheduleUpdatedEventHandler(IMediator mediator)
    : INotificationHandler<DoctorScheduleUpdatedEvent>
{
    private const int GenerationHorizonDays = 30;

    public async Task Handle(DoctorScheduleUpdatedEvent notification, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var until = today.AddDays(GenerationHorizonDays);

        await mediator.Send(new GenerateSlotsCommand(notification.DoctorId, today, until), ct);
    }
}
