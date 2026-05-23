using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Repositories;
using ClinicService.Domain.ValueObjects;

namespace ClinicService.Application.Clinics.Commands.CreateClinic;

internal sealed class CreateClinicCommandHandler(
    IClinicRepository clinicRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateClinicCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateClinicCommand request, CancellationToken ct)
    {
        var addressResult = Address.Create(request.City, request.Street, request.BuildingNumber);
        if (addressResult.IsFailure)
            return Result.Failure<Guid>(addressResult.Error);

        var clinic = Clinic.Create(
            request.Name,
            addressResult.Value,
            request.Phone,
            request.TimeZoneId,
            request.Description,
            request.PhotoUrl);

        await clinicRepository.AddAsync(clinic, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return clinic.Id;
    }
}
