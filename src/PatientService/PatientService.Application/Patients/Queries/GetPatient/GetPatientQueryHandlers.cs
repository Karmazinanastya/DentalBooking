using MediatR;
using Shared.BuildingBlocks.Common;
using PatientService.Domain.Aggregates;
using PatientService.Domain.Repositories;

namespace PatientService.Application.Patients.Queries.GetPatient;

internal sealed class GetPatientByIdQueryHandler(IPatientRepository patientRepository)
    : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken ct)
    {
        var patient = await patientRepository.GetByIdAsync(request.PatientId, ct);
        if (patient is null)
            return Result.Failure<PatientDto>(Error.NotFound(nameof(Patient), request.PatientId));

        return PatientMapping.ToDto(patient);
    }
}

internal sealed class GetPatientByChatIdQueryHandler(IPatientRepository patientRepository)
    : IRequestHandler<GetPatientByChatIdQuery, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(GetPatientByChatIdQuery request, CancellationToken ct)
    {
        var patient = await patientRepository.GetByChatIdAsync(request.ChatId, ct);
        if (patient is null)
            return Result.Failure<PatientDto>(Error.NotFound(nameof(Patient), request.ChatId));

        return PatientMapping.ToDto(patient);
    }
}

file static class PatientMapping
{
    internal static PatientDto ToDto(Patient p) => new(
        p.Id, p.ChatId, p.FirstName, p.LastName, p.FullName, p.PhoneNumber, p.RegisteredAtUtc);
}
