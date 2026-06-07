using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Doctors.Queries.GetDoctorsByClinic;

public sealed record GetDoctorsByClinicQuery(Guid? ClinicId, Guid? ServiceId = null)
    : IRequest<Result<IReadOnlyList<DoctorListDto>>>;
