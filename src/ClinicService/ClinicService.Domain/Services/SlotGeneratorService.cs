using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Entities;

namespace ClinicService.Domain.Services;

public sealed class SlotGeneratorService
{
    private const int SlotStepMinutes = 60;

    public IReadOnlyList<TimeSlot> GenerateSlots(
        Doctor doctor,
        Clinic clinic,
        DateOnly fromDate,
        DateOnly toDate)
    {
        var timeZone = TimeZoneHelper.Find(clinic.TimeZoneId);
        var blockedDates = doctor.ScheduleBlocks.Select(b => b.Date).ToHashSet();
        var slots = new List<TimeSlot>();

        for (var date = fromDate; date <= toDate; date = date.AddDays(1))
        {
            if (blockedDates.Contains(date)) continue;

            var template = doctor.ScheduleTemplates
                .FirstOrDefault(t => t.DayOfWeek == date.DayOfWeek && t.IsActive);

            if (template is null) continue;

            var current = template.WorkingHours.Start;
            var end = template.WorkingHours.End;

            while (current.AddMinutes(SlotStepMinutes) <= end)
            {
                var isLunch = template.LunchBreak is not null &&
                              template.LunchBreak.Contains(current);

                if (!isLunch)
                {
                    var localStart = date.ToDateTime(current);
                    var startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone);
                    var endUtc = startUtc.AddMinutes(SlotStepMinutes);

                    slots.Add(TimeSlot.Create(doctor.Id, clinic.Id, startUtc, endUtc));
                }

                current = current.AddMinutes(SlotStepMinutes);
            }
        }

        return slots;
    }
}
