using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Services.Commands.CreateService;

public sealed record CreateServiceCommand(
    string Name,
    string Category,
    int DurationMinutes,
    decimal Price,
    string? Description
) : IRequest<Result<Guid>>;
