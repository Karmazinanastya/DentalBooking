using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Repositories;
using ClinicService.Domain.ValueObjects;

namespace ClinicService.Application.Doctors.Commands.SetSchedule;

internal sealed class SetDoctorScheduleCommandHandler(
    IDoctorRepository doctorRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SetDoctorScheduleCommand, Result>
{
    public async Task<Result> Handle(SetDoctorScheduleCommand request, CancellationToken ct)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, ct);
        if (doctor is null)
            return Result.Failure(Error.NotFound(nameof(Doctor), request.DoctorId));

        var workingHoursResult = WorkingHours.Create(request.WorkStart, request.WorkEnd);
        if (workingHoursResult.IsFailure)
            return Result.Failure(workingHoursResult.Error);

        WorkingHours? lunchBreak = null;
        if (request.LunchStart.HasValue && request.LunchEnd.HasValue)
        {
            var lunchResult = WorkingHours.Create(request.LunchStart.Value, request.LunchEnd.Value);
            if (lunchResult.IsFailure)
                return Result.Failure(lunchResult.Error);
            lunchBreak = lunchResult.Value;
        }

        var result = doctor.SetScheduleTemplate(request.DayOfWeek, workingHoursResult.Value, lunchBreak);
        if (result.IsFailure)
            return result;

        doctorRepository.Update(doctor);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
