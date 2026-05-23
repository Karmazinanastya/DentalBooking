namespace NotificationService.Application.Abstractions;

public interface IScheduler
{
    void ScheduleReminder(Guid appointmentId, long chatId, DateTime sendAtUtc, string message);
    void CancelReminders(Guid appointmentId);
}
