using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Doctors.Commands.SetSchedule;

public sealed record SetDoctorScheduleCommand(
    Guid DoctorId,
    DayOfWeek DayOfWeek,
    TimeOnly WorkStart,
    TimeOnly WorkEnd,
    TimeOnly? LunchStart,
    TimeOnly? LunchEnd
) : IRequest<Result>;
