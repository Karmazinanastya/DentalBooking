using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Doctors.Queries.GetDoctorsByClinic;

public sealed record GetDoctorsByClinicQuery(Guid ClinicId)
    : IRequest<Result<IReadOnlyList<DoctorListDto>>>;
