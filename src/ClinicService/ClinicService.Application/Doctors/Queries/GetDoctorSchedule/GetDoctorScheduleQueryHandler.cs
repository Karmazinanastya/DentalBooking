using MediatR;
using Shared.BuildingBlocks.Common;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Doctors.Queries.GetDoctorSchedule;

internal sealed class GetDoctorScheduleQueryHandler(IDoctorRepository doctorRepository)
    : IRequestHandler<GetDoctorScheduleQuery, Result<DoctorScheduleDto>>
{
    public async Task<Result<DoctorScheduleDto>> Handle(
        GetDoctorScheduleQuery request, CancellationToken ct)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, ct);
        if (doctor is null)
            return Result.Failure<DoctorScheduleDto>(Error.NotFound(nameof(Doctor), request.DoctorId));

        var days = doctor.ScheduleTemplates
            .OrderBy(t => t.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)t.DayOfWeek)
            .Select(t => new ScheduleDayDto(
                t.DayOfWeek,
                t.WorkStart.ToString("HH:mm"),
                t.WorkEnd.ToString("HH:mm"),
                t.LunchStart?.ToString("HH:mm"),
                t.LunchEnd?.ToString("HH:mm")))
            .ToList();

        return new DoctorScheduleDto(doctor.Id, doctor.FullName, days);
    }
}
