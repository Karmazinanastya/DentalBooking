using MediatR;
using Shared.BuildingBlocks.Common;

namespace PatientService.Application.Patients.Commands.UpdatePatient;

public sealed record UpdatePatientCommand(
    Guid PatientId,
    string FirstName,
    string LastName
) : IRequest<Result>;
