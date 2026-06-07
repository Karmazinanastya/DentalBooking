using MediatR;
using Shared.BuildingBlocks.Common;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Services.Queries.GetServices;

internal sealed class GetServicesQueryHandler(IServiceRepository serviceRepository)
    : IRequestHandler<GetServicesQuery, Result<IReadOnlyList<ServiceDto>>>
{
    public async Task<Result<IReadOnlyList<ServiceDto>>> Handle(GetServicesQuery request, CancellationToken ct)
    {
        var services = await serviceRepository.GetAllActiveAsync(ct);
        IReadOnlyList<ServiceDto> dtos = services.Select(s => new ServiceDto(
            s.Id, s.Name, s.Category, s.Description, s.DurationMinutes, s.Price, s.IsActive)).ToList();
        return Result.Success(dtos);
    }
}
