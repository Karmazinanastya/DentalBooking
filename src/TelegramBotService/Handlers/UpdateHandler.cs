using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotService.HttpClients;
using TelegramBotService.Keyboards;
using TelegramBotService.Session;

namespace TelegramBotService.Handlers;

public sealed class UpdateHandler(
    ITelegramBotClient bot,
    SessionService sessions,
    PatientApiClient patientApi,
    ClinicApiClient clinicApi,
    BookingApiClient bookingApi,
    ILogger<UpdateHandler> logger)
{
    public async Task HandleAsync(Update update, CancellationToken ct)
    {
        try
        {
            await (update.Type switch
            {
                UpdateType.Message when update.Message is not null
                    => HandleMessageAsync(update.Message, ct),
                UpdateType.CallbackQuery when update.CallbackQuery is not null
                    => HandleCallbackAsync(update.CallbackQuery, ct),
                _ => Task.CompletedTask
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling update {UpdateId}", update.Id);
        }
    }

    private async Task HandleMessageAsync(Message msg, CancellationToken ct)
    {
        var chatId = msg.Chat.Id;

        if (msg.Contact is not null)
        {
            await HandleContactAsync(msg, ct);
            return;
        }

        var text = msg.Text ?? string.Empty;

        if (text == "/start")
        {
            await HandleStartAsync(msg, ct);
            return;
        }

        var session = await sessions.GetOrCreateAsync(chatId);

        if (session.State == BotState.AwaitingPhone)
        {
            await bot.SendMessage(chatId,
                "Будь ласка, натисніть кнопку нижче, щоб надіслати номер телефону.",
                replyMarkup: BotKeyboards.RequestContact, cancellationToken: ct);
            return;
        }

        switch (text)
        {
            case "📅 Записатися":
                await StartBookingFlowAsync(chatId, session, ct);
                break;
            case "📋 Мої записи":
                await ShowMyAppointmentsAsync(chatId, session, ct);
                break;
            case "❌ Скасувати запис":
                await StartCancellationFlowAsync(chatId, session, ct);
                break;
            default:
                await bot.SendMessage(chatId, "Оберіть дію з меню.",
                    replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
                break;
        }
    }

    private async Task HandleStartAsync(Message msg, CancellationToken ct)
    {
        var chatId = msg.Chat.Id;
        var patient = await patientApi.GetByChatIdAsync(chatId, ct);

        if (patient is not null)
        {
            var session = await sessions.GetOrCreateAsync(chatId);
            session.PatientId = patient.Id;
            session.State = BotState.Idle;
            await sessions.SaveAsync(chatId, session);

            await bot.SendMessage(chatId,
                $"З поверненням, {patient.FullName}! Чим можу допомогти?",
                replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
            return;
        }

        var firstName = msg.From?.FirstName ?? "Пацієнт";
        var lastName = msg.From?.LastName ?? string.Empty;

        var newSession = new BotSession { State = BotState.AwaitingPhone };
        await sessions.SaveAsync(chatId, newSession);

        await bot.SendMessage(chatId,
            $"Ласкаво просимо до DentalBook, {firstName}!\n\n" +
            "Для реєстрації нам потрібен ваш номер телефону.",
            replyMarkup: BotKeyboards.RequestContact, cancellationToken: ct);
    }

    private async Task HandleContactAsync(Message msg, CancellationToken ct)
    {
        var chatId = msg.Chat.Id;
        var contact = msg.Contact!;
        var session = await sessions.GetOrCreateAsync(chatId);

        if (session.State != BotState.AwaitingPhone)
            return;

        var firstName = msg.From?.FirstName ?? contact.FirstName;
        var lastName = msg.From?.LastName ?? contact.LastName ?? string.Empty;
        var phone = contact.PhoneNumber;

        var patientId = await patientApi.RegisterAsync(
            new RegisterPatientRequest(chatId, firstName, lastName, phone), ct);

        session.PatientId = patientId;
        session.State = BotState.Idle;
        await sessions.SaveAsync(chatId, session);

        await bot.SendMessage(chatId,
            "Реєстрацію завершено! Тепер ви можете записатися на прийом.",
            replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
    }

    private async Task StartBookingFlowAsync(long chatId, BotSession session, CancellationToken ct)
    {
        if (session.PatientId == Guid.Empty)
        {
            await bot.SendMessage(chatId,
                "Спочатку потрібно зареєструватися. Введіть /start.", cancellationToken: ct);
            return;
        }

        var clinics = await clinicApi.GetClinicsAsync(ct);
        if (clinics.Count == 0)
        {
            await bot.SendMessage(chatId, "Наразі клініки недоступні. Спробуйте пізніше.",
                replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
            return;
        }

        session.State = BotState.SelectingClinic;
        await sessions.SaveAsync(chatId, session);

        await bot.SendMessage(chatId, "Оберіть клініку:",
            replyMarkup: BotKeyboards.FromClinics(clinics), cancellationToken: ct);
    }

    private async Task ShowMyAppointmentsAsync(long chatId, BotSession session, CancellationToken ct)
    {
        if (session.PatientId == Guid.Empty)
        {
            await bot.SendMessage(chatId, "Спочатку потрібно зареєструватися. Введіть /start.", cancellationToken: ct);
            return;
        }

        var appointments = await bookingApi.GetMyAppointmentsAsync(session.PatientId, ct);
        if (appointments.Count == 0)
        {
            await bot.SendMessage(chatId, "У вас немає записів.",
                replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
            return;
        }

        var text = string.Join("\n\n", appointments.Select((a, i) =>
            $"{i + 1}. {a.LocalDateTime}\n" +
            $"   Клініка: {a.ClinicName}\n" +
            $"   Лікар: {a.DoctorFullName}\n" +
            $"   Послуга: {a.ServiceName}\n" +
            $"   Статус: {a.Status}"));

        await bot.SendMessage(chatId, $"Ваші записи:\n\n{text}",
            replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
    }

    private async Task StartCancellationFlowAsync(long chatId, BotSession session, CancellationToken ct)
    {
        if (session.PatientId == Guid.Empty)
        {
            await bot.SendMessage(chatId, "Спочатку потрібно зареєструватися. Введіть /start.", cancellationToken: ct);
            return;
        }

        var appointments = await bookingApi.GetMyAppointmentsAsync(session.PatientId, ct);
        var active = appointments
            .Where(a => a.Status is "Confirmed" or "Pending")
            .ToList();

        if (active.Count == 0)
        {
            await bot.SendMessage(chatId, "У вас немає активних записів для скасування.",
                replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
            return;
        }

        session.State = BotState.SelectingAppointmentToCancel;
        await sessions.SaveAsync(chatId, session);

        await bot.SendMessage(chatId, "Оберіть запис для скасування:",
            replyMarkup: BotKeyboards.FromAppointments(active), cancellationToken: ct);
    }

    private async Task HandleCallbackAsync(CallbackQuery query, CancellationToken ct)
    {
        var chatId = query.Message!.Chat.Id;
        var data = query.Data ?? string.Empty;
        var session = await sessions.GetOrCreateAsync(chatId);

        await bot.AnswerCallbackQuery(query.Id, cancellationToken: ct);

        if (data.StartsWith("clinic_") && session.State == BotState.SelectingClinic)
        {
            var clinicId = Guid.Parse(data[7..]);
            session.SelectedClinicId = clinicId;
            session.State = BotState.SelectingDoctor;
            await sessions.SaveAsync(chatId, session);

            var doctors = await clinicApi.GetDoctorsByClinicAsync(clinicId, ct);
            if (doctors.Count == 0)
            {
                await bot.SendMessage(chatId, "У цій клініці немає доступних лікарів. Оберіть іншу.",
                    replyMarkup: await GetClinicsKeyboardAsync(ct), cancellationToken: ct);
                session.State = BotState.SelectingClinic;
                await sessions.SaveAsync(chatId, session);
                return;
            }

            await bot.SendMessage(chatId, "Оберіть лікаря:",
                replyMarkup: BotKeyboards.FromDoctors(doctors), cancellationToken: ct);
            return;
        }

        if (data.StartsWith("doctor_") && session.State == BotState.SelectingDoctor)
        {
            var parts = data[7..].Split('|');
            session.SelectedDoctorId = Guid.Parse(parts[0]);
            session.SelectedDoctorName = parts.Length > 1 ? parts[1] : string.Empty;
            session.State = BotState.SelectingDate;
            await sessions.SaveAsync(chatId, session);

            await bot.SendMessage(chatId, "Оберіть дату:",
                replyMarkup: BotKeyboards.DatePicker(), cancellationToken: ct);
            return;
        }

        if (data.StartsWith("date_") && session.State == BotState.SelectingDate)
        {
            var date = DateOnly.Parse(data[5..]);
            session.SelectedDate = date;
            session.State = BotState.SelectingSlot;
            await sessions.SaveAsync(chatId, session);

            var slots = await clinicApi.GetAvailableSlotsAsync(session.SelectedDoctorId!.Value, date, ct);
            if (slots.Count == 0)
            {
                await bot.SendMessage(chatId, "На цю дату немає вільних слотів. Оберіть іншу дату:",
                    replyMarkup: BotKeyboards.DatePicker(), cancellationToken: ct);
                session.State = BotState.SelectingDate;
                await sessions.SaveAsync(chatId, session);
                return;
            }

            await bot.SendMessage(chatId, "Оберіть час:",
                replyMarkup: BotKeyboards.FromSlots(slots), cancellationToken: ct);
            return;
        }

        if (data.StartsWith("slot_") && session.State == BotState.SelectingSlot)
        {
            var slotParts = data[5..].Split('|');
            session.SelectedSlotId = Guid.Parse(slotParts[0]);
            session.SelectedSlotTime = slotParts.Length > 1 ? slotParts[1] : null;
            session.State = BotState.ConfirmingBooking;
            await sessions.SaveAsync(chatId, session);

            var summary =
                $"Підтвердіть запис:\n\n" +
                $"Лікар: {session.SelectedDoctorName}\n" +
                $"Дата: {session.SelectedDate:dd.MM.yyyy}\n" +
                $"Час: {session.SelectedSlotTime ?? "обраний"}";

            await bot.SendMessage(chatId, summary,
                replyMarkup: BotKeyboards.ConfirmCancel("confirm_yes", "confirm_no"),
                cancellationToken: ct);
            return;
        }

        if (data == "confirm_yes" && session.State == BotState.ConfirmingBooking)
        {
            await ConfirmBookingAsync(chatId, session, ct);
            return;
        }

        if (data == "confirm_no" && session.State == BotState.ConfirmingBooking)
        {
            session.State = BotState.Idle;
            await sessions.SaveAsync(chatId, session);
            await bot.SendMessage(chatId, "Запис скасовано.",
                replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
            return;
        }

        if (data.StartsWith("cancel_") && data != "cancel_yes" && data != "cancel_no"
            && session.State == BotState.SelectingAppointmentToCancel)
        {
            session.SelectedAppointmentId = Guid.Parse(data[7..]);
            session.State = BotState.ConfirmingCancellation;
            await sessions.SaveAsync(chatId, session);

            await bot.SendMessage(chatId, "Ви впевнені, що хочете скасувати цей запис?",
                replyMarkup: BotKeyboards.ConfirmCancel("cancel_yes", "cancel_no"),
                cancellationToken: ct);
            return;
        }

        if (data == "cancel_yes" && session.State == BotState.ConfirmingCancellation)
        {
            await bookingApi.CancelByPatientAsync(session.SelectedAppointmentId!.Value, session.PatientId, ct);
            session.State = BotState.Idle;
            await sessions.SaveAsync(chatId, session);
            await bot.SendMessage(chatId, "Запис успішно скасовано.",
                replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
            return;
        }

        if (data == "cancel_no" && session.State == BotState.ConfirmingCancellation)
        {
            session.State = BotState.Idle;
            await sessions.SaveAsync(chatId, session);
            await bot.SendMessage(chatId, "Скасування відмінено.",
                replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
        }
    }

    private async Task ConfirmBookingAsync(long chatId, BotSession session, CancellationToken ct)
    {
        var appointmentId = await bookingApi.CreateAppointmentAsync(
            new CreateAppointmentRequest(session.PatientId, chatId, session.SelectedSlotId!.Value), ct);

        await bookingApi.ConfirmAsync(appointmentId, session.PatientId, ct);

        session.State = BotState.Idle;
        await sessions.SaveAsync(chatId, session);

        await bot.SendMessage(chatId,
            "Запис успішно підтверджено! Ви отримаєте сповіщення з деталями.",
            replyMarkup: BotKeyboards.MainMenu, cancellationToken: ct);
    }

    private async Task<InlineKeyboardMarkup> GetClinicsKeyboardAsync(CancellationToken ct)
    {
        var clinics = await clinicApi.GetClinicsAsync(ct);
        return BotKeyboards.FromClinics(clinics);
    }
}
