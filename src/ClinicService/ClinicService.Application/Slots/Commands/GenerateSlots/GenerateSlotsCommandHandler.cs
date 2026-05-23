using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Repositories;
using ClinicService.Domain.Services;

namespace ClinicService.Application.Slots.Commands.GenerateSlots;

internal sealed class GenerateSlotsCommandHandler(
    IDoctorRepository doctorRepository,
    IClinicRepository clinicRepository,
    ITimeSlotRepository timeSlotRepository,
    SlotGeneratorService slotGenerator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GenerateSlotsCommand, Result<int>>
{
    public async Task<Result<int>> Handle(GenerateSlotsCommand request, CancellationToken ct)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, ct);
        if (doctor is null)
            return Result.Failure<int>(Error.NotFound(nameof(Doctor), request.DoctorId));

        var clinic = await clinicRepository.GetByIdAsync(doctor.ClinicId, ct);
        if (clinic is null)
            return Result.Failure<int>(Error.NotFound(nameof(Clinic), doctor.ClinicId));

        var slots = slotGenerator.GenerateSlots(doctor, clinic, request.FromDate, request.ToDate);

        await timeSlotRepository.AddRangeAsync(slots, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return slots.Count;
    }
}
