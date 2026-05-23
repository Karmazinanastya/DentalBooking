using MediatR;
using Shared.BuildingBlocks.Common;

namespace PatientService.Application.Patients.Commands.RegisterPatient;

public sealed record RegisterPatientCommand(
    long ChatId,
    string FirstName,
    string LastName,
    string PhoneNumber
) : IRequest<Result<Guid>>;
