using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Doctors.Queries.GetDoctorSchedule;

public sealed record GetDoctorScheduleQuery(Guid DoctorId)
    : IRequest<Result<DoctorScheduleDto>>;

public sealed record DoctorScheduleDto(
    Guid DoctorId,
    string FullName,
    IReadOnlyList<ScheduleDayDto> Days);

public sealed record ScheduleDayDto(
    DayOfWeek DayOfWeek,
    string WorkStart,
    string WorkEnd,
    string? LunchStart,
    string? LunchEnd);
