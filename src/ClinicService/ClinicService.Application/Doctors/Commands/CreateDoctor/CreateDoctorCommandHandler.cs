using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Doctors.Commands.CreateDoctor;

internal sealed class CreateDoctorCommandHandler(
    IClinicRepository clinicRepository,
    IDoctorRepository doctorRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDoctorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateDoctorCommand request, CancellationToken ct)
    {
        var clinic = await clinicRepository.GetByIdAsync(request.ClinicId, ct);
        if (clinic is null)
            return Result.Failure<Guid>(Error.NotFound(nameof(Clinic), request.ClinicId));

        var doctor = Doctor.Create(
            request.ClinicId,
            request.FirstName,
            request.LastName,
            request.Specialization,
            request.PhotoUrl,
            request.Bio);

        await doctorRepository.AddAsync(doctor, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return doctor.Id;
    }
}
