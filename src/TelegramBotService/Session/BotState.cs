namespace TelegramBotService.Session;

public enum BotState
{
    Idle,
    AwaitingPhone,
    SelectingClinic,
    SelectingService,
    SelectingDoctor,
    SelectingDate,
    SelectingSlot,
    ConfirmingBooking,
    SelectingAppointmentToCancel,
    ConfirmingCancellation,
    SelectingAppointmentToReschedule,
    ReschedulingDate,
    ReschedulingSlot,
    ConfirmingReschedule
}
