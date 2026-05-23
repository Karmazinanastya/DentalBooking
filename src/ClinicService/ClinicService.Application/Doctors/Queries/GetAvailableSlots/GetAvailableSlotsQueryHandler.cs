using MediatR;
using Shared.BuildingBlocks.Common;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Doctors.Queries.GetAvailableSlots;

internal sealed class GetAvailableSlotsQueryHandler(
    IDoctorRepository doctorRepository,
    IClinicRepository clinicRepository,
    ITimeSlotRepository timeSlotRepository)
    : IRequestHandler<GetAvailableSlotsQuery, Result<IReadOnlyList<SlotDto>>>
{
    public async Task<Result<IReadOnlyList<SlotDto>>> Handle(GetAvailableSlotsQuery request, CancellationToken ct)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, ct);
        if (doctor is null)
            return Result.Failure<IReadOnlyList<SlotDto>>(Error.NotFound(nameof(Doctor), request.DoctorId));

        var clinic = await clinicRepository.GetByIdAsync(doctor.ClinicId, ct);
        if (clinic is null)
            return Result.Failure<IReadOnlyList<SlotDto>>(Error.NotFound(nameof(Clinic), doctor.ClinicId));

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(clinic.TimeZoneId);
        var slots = await timeSlotRepository.GetAvailableAsync(request.DoctorId, request.Date, ct);

        var dtos = slots.Select(s => new SlotDto(
            s.Id,
            s.StartUtc,
            s.EndUtc,
            TimeZoneInfo.ConvertTimeFromUtc(s.StartUtc, timeZone).ToString("HH:mm")
        )).ToList();

        return dtos;
    }
}
