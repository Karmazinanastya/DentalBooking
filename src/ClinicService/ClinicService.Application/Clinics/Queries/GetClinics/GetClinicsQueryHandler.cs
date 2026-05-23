using MediatR;
using Shared.BuildingBlocks.Common;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Clinics.Queries.GetClinics;

internal sealed class GetClinicsQueryHandler(IClinicRepository clinicRepository)
    : IRequestHandler<GetClinicsQuery, Result<IReadOnlyList<ClinicDto>>>
{
    public async Task<Result<IReadOnlyList<ClinicDto>>> Handle(GetClinicsQuery request, CancellationToken ct)
    {
        var clinics = string.IsNullOrWhiteSpace(request.City)
            ? await clinicRepository.GetAllActiveAsync(ct)
            : await clinicRepository.GetByCityAsync(request.City, ct);

        var dtos = clinics.Select(c => new ClinicDto(
            c.Id,
            c.Name,
            c.Address.City,
            c.Address.Street,
            c.Address.BuildingNumber,
            c.Phone,
            c.Description,
            c.PhotoUrl,
            c.TimeZoneId
        )).ToList();

        return dtos;
    }
}
