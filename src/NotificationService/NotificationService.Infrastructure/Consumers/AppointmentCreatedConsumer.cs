using MassTransit;
using NotificationService.Application.Abstractions;
using Shared.Contracts.IntegrationEvents.Appointments;

namespace NotificationService.Infrastructure.Consumers;

internal sealed class AppointmentCreatedConsumer(ITelegramSender sender, IScheduler scheduler)
    : IConsumer<AppointmentCreatedEvent>
{
    public async Task Consume(ConsumeContext<AppointmentCreatedEvent> context)
    {
        var msg = context.Message;
        var localTime = ConvertToLocalTime(msg.AppointmentDateUtc, msg.ClinicTimeZoneId);

        await sender.SendTextAsync(msg.PatientChatId,
            $"Запис підтверджено!\n\n" +
            $"Клініка: {msg.ClinicName}\n" +
            $"Адреса: {msg.ClinicAddress}\n" +
            $"Лікар: {msg.DoctorFullName}\n" +
            $"Послуга: {msg.ServiceName}\n" +
            $"Час: {localTime}",
            context.CancellationToken);

        scheduler.ScheduleReminder(msg.AppointmentId, msg.PatientChatId,
            msg.AppointmentDateUtc.AddHours(-24),
            $"Нагадування: завтра о {localTime} у вас прийом у {msg.ClinicName} ({msg.ClinicAddress}).");

        scheduler.ScheduleReminder(msg.AppointmentId, msg.PatientChatId,
            msg.AppointmentDateUtc.AddHours(-1),
            $"Нагадування: через 1 годину о {localTime} у вас прийом у {msg.ClinicName}.");
    }

    private static string ConvertToLocalTime(DateTime utc, string timeZoneId)
    {
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utc, tz).ToString("dd.MM.yyyy HH:mm");
        }
        catch
        {
            return utc.ToString("dd.MM.yyyy HH:mm") + " UTC";
        }
    }
}
