using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Clinics.Commands.CreateClinic;

public sealed record CreateClinicCommand(
    string Name,
    string City,
    string Street,
    string BuildingNumber,
    string Phone,
    string TimeZoneId,
    string? Description,
    string? PhotoUrl
) : IRequest<Result<Guid>>;
