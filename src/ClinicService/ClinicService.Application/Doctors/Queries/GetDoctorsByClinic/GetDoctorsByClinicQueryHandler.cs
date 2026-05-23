using MediatR;
using Shared.BuildingBlocks.Common;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Doctors.Queries.GetDoctorsByClinic;

internal sealed class GetDoctorsByClinicQueryHandler(IDoctorRepository doctorRepository)
    : IRequestHandler<GetDoctorsByClinicQuery, Result<IReadOnlyList<DoctorListDto>>>
{
    public async Task<Result<IReadOnlyList<DoctorListDto>>> Handle(
        GetDoctorsByClinicQuery request, CancellationToken ct)
    {
        var doctors = await doctorRepository.GetByClinicAsync(request.ClinicId, ct);
        IReadOnlyList<DoctorListDto> dtos = doctors
            .Select(d => new DoctorListDto(d.Id, d.FullName, d.Specialization))
            .ToList();
        return Result.Success(dtos);
    }
}
