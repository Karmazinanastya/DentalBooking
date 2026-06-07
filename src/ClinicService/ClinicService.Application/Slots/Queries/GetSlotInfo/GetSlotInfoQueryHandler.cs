using MediatR;
using Shared.BuildingBlocks.Common;
using ClinicService.Domain.Entities;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Slots.Queries.GetSlotInfo;

internal sealed class GetSlotInfoQueryHandler(
    ITimeSlotRepository timeSlotRepository,
    IDoctorRepository doctorRepository,
    IClinicRepository clinicRepository)
    : IRequestHandler<GetSlotInfoQuery, Result<SlotInfoDto>>
{
    public async Task<Result<SlotInfoDto>> Handle(GetSlotInfoQuery request, CancellationToken ct)
    {
        var slot = await timeSlotRepository.GetByIdAsync(request.SlotId, ct);
        if (slot is null)
            return Result.Failure<SlotInfoDto>(Error.NotFound(nameof(TimeSlot), request.SlotId));

        var doctor = await doctorRepository.GetByIdAsync(slot.DoctorId, ct);
        if (doctor is null)
            return Result.Failure<SlotInfoDto>(Error.NotFound("Doctor", slot.DoctorId));

        var clinic = await clinicRepository.GetByIdAsync(slot.ClinicId, ct);
        if (clinic is null)
            return Result.Failure<SlotInfoDto>(Error.NotFound("Clinic", slot.ClinicId));

        return new SlotInfoDto(
            slot.Id,
            doctor.Id,
            doctor.FullName,
            clinic.Id,
            clinic.Name,
            clinic.Address.ToString(),
            clinic.TimeZoneId,
            string.Empty,
            slot.StartUtc);
    }
}
