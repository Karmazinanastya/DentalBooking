using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using PatientService.Domain.Aggregates;
using PatientService.Domain.Repositories;

namespace PatientService.Application.Patients.Commands.UpdatePatient;

internal sealed class UpdatePatientCommandHandler(
    IPatientRepository patientRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePatientCommand, Result>
{
    public async Task<Result> Handle(UpdatePatientCommand request, CancellationToken ct)
    {
        var patient = await patientRepository.GetByIdAsync(request.PatientId, ct);
        if (patient is null)
            return Result.Failure(Error.NotFound(nameof(Patient), request.PatientId));

        patient.Update(request.FirstName, request.LastName);

        patientRepository.Update(patient);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
