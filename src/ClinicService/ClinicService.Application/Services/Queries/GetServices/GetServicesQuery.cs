using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Services.Queries.GetServices;

public sealed record GetServicesQuery : IRequest<Result<IReadOnlyList<ServiceDto>>>;
