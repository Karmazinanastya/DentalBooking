using Hangfire;
using NotificationService.Application.Abstractions;

namespace NotificationService.Infrastructure.Scheduling;

internal sealed class HangfireScheduler : IScheduler
{
    public void ScheduleReminder(Guid appointmentId, long chatId, DateTime sendAtUtc, string message)
    {
        var delay = sendAtUtc - DateTime.UtcNow;
        if (delay <= TimeSpan.Zero)
            return;

        BackgroundJob.Schedule<ReminderJob>(j => j.ExecuteAsync(chatId, message), delay);
    }

    public void CancelReminders(Guid appointmentId) { }
}
