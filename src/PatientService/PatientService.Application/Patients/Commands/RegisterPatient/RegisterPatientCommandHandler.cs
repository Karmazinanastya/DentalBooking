using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using PatientService.Domain.Aggregates;
using PatientService.Domain.Repositories;

namespace PatientService.Application.Patients.Commands.RegisterPatient;

internal sealed class RegisterPatientCommandHandler(
    IPatientRepository patientRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterPatientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterPatientCommand request, CancellationToken ct)
    {
        var existing = await patientRepository.GetExistingAsync(request.ChatId, request.PhoneNumber, ct);
        if (existing is not null)
            return existing.Id;

        var patientResult = Patient.Create(
            request.ChatId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber);

        if (patientResult.IsFailure)
            return Result.Failure<Guid>(patientResult.Error);

        await patientRepository.AddAsync(patientResult.Value, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return patientResult.Value.Id;
    }
}
