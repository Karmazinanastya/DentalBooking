using MediatR;
using Shared.BuildingBlocks.Common;

namespace PatientService.Application.Patients.Queries.GetPatient;

public sealed record GetPatientByIdQuery(Guid PatientId) : IRequest<Result<PatientDto>>;

public sealed record GetPatientByChatIdQuery(long ChatId) : IRequest<Result<PatientDto>>;
