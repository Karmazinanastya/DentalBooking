using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotService.HttpClients;

namespace TelegramBotService.Keyboards;

public static class BotKeyboards
{
    public static readonly ReplyKeyboardMarkup MainMenu = new(
    [
        [new KeyboardButton("📅 Запис"), new KeyboardButton("🦷 Послуги")]
    ])
    {
        ResizeKeyboard = true
    };

    public static readonly ReplyKeyboardMarkup BookingSubmenu = new(
    [
        [new KeyboardButton("📅 Запис на прийом")],
        [new KeyboardButton("🔄 Перенесення запису")],
        [new KeyboardButton("❌ Скасування запису")],
        [new KeyboardButton("⬅️ Назад")]
    ])
    {
        ResizeKeyboard = true
    };

    public static readonly ReplyKeyboardMarkup ServicesSubmenu = new(
    [
        [new KeyboardButton("📜 Перегляд історії")],
        [new KeyboardButton("👨‍⚕️ Перегляд лікарів")],
        [new KeyboardButton("🦷 Перегляд послуг")],
        [new KeyboardButton("⬅️ Назад")]
    ])
    {
        ResizeKeyboard = true
    };

    public static readonly ReplyKeyboardMarkup RequestContact = new(
    [
        [KeyboardButton.WithRequestContact("📱 Надіслати номер телефону")]
    ])
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = true
    };

    public static InlineKeyboardMarkup FromClinics(IReadOnlyList<ClinicResponse> clinics) =>
        new(clinics.Select(c => new[]
        {
            InlineKeyboardButton.WithCallbackData($"🏥 {c.Name} — {c.City}", $"clinic_{c.Id}")
        }));

    public static InlineKeyboardMarkup FromServices(IReadOnlyList<ServiceResponse> services) =>
        new(services.Select(s => new[]
        {
            InlineKeyboardButton.WithCallbackData($"🦷 {s.Name} ({s.Category})", $"service_{s.Id}")
        }));

    public static InlineKeyboardMarkup FromDoctors(IReadOnlyList<DoctorResponse> doctors) =>
        new(doctors.Select(d => new[]
        {
            InlineKeyboardButton.WithCallbackData($"👨‍⚕️ {d.FullName} ({d.Specialization})", $"doctor_{d.Id}")
        }));

    public static InlineKeyboardMarkup DatePicker(int daysAhead = 7)
    {
        var buttons = Enumerable.Range(1, daysAhead)
            .Select(i => DateOnly.FromDateTime(DateTime.Today.AddDays(i)))
            .Select(d => new[] { InlineKeyboardButton.WithCallbackData(d.ToString("ddd dd.MM"), $"date_{d:yyyy-MM-dd}") })
            .ToArray();
        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup FromSlots(IReadOnlyList<SlotResponse> slots) =>
        new(slots.Select(s => new[]
        {
            InlineKeyboardButton.WithCallbackData($"🕐 {s.LocalTime}", $"slot_{s.SlotId}|{s.LocalTime}")
        }));

    public static InlineKeyboardMarkup FromAppointments(IReadOnlyList<AppointmentResponse> appointments) =>
        new(appointments.Select(a => new[]
        {
            InlineKeyboardButton.WithCallbackData(
                $"{a.LocalDateTime} — {a.DoctorFullName}", $"cancel_{a.Id}")
        }));

    public static InlineKeyboardMarkup ConfirmCancel(string yesData, string noData) =>
        new([[
            InlineKeyboardButton.WithCallbackData("✅ Так", yesData),
            InlineKeyboardButton.WithCallbackData("❌ Ні", noData)
        ]]);
}
