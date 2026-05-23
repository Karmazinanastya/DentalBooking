namespace TelegramBotService.Session;

public enum BotState
{
    Idle,
    AwaitingPhone,
    SelectingClinic,
    SelectingDoctor,
    SelectingDate,
    SelectingSlot,
    ConfirmingBooking,
    SelectingAppointmentToCancel,
    ConfirmingCancellation
}
