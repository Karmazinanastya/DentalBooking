using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Extensions;
using ClinicService.Application.Slots.Commands.BookSlot;
using ClinicService.Application.Slots.Commands.ReleaseSlot;
using ClinicService.Application.Slots.Commands.ReserveSlot;
using ClinicService.Application.Slots.Queries.GetSlotInfo;

namespace ClinicService.API.Controllers;

[ApiController]
[Route("slots")]
public sealed class SlotsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{slotId:guid}")]
    public async Task<IActionResult> GetSlotInfo(Guid slotId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSlotInfoQuery(slotId), ct);
        return result.ToActionResult();
    }

    [HttpPut("{slotId:guid}/reserve")]
    public async Task<IActionResult> Reserve(Guid slotId, CancellationToken ct)
    {
        var result = await mediator.Send(new ReserveSlotCommand(slotId), ct);
        return result.ToActionResult();
    }

    [HttpPut("{slotId:guid}/book")]
    public async Task<IActionResult> Book(Guid slotId, CancellationToken ct)
    {
        var result = await mediator.Send(new BookSlotCommand(slotId), ct);
        return result.ToActionResult();
    }

    [HttpPut("{slotId:guid}/release")]
    public async Task<IActionResult> Release(Guid slotId, CancellationToken ct)
    {
        var result = await mediator.Send(new ReleaseSlotCommand(slotId), ct);
        return result.ToActionResult();
    }
}
