namespace TelegramBotService.Session;

public sealed class BotSession
{
    public BotState State { get; set; } = BotState.Idle;
    public Guid PatientId { get; set; }
    public Guid? SelectedClinicId { get; set; }
    public string? SelectedClinicName { get; set; }
    public Guid? SelectedServiceId { get; set; }
    public string? SelectedServiceName { get; set; }
    public Guid? SelectedDoctorId { get; set; }
    public string? SelectedDoctorName { get; set; }
    public DateOnly? SelectedDate { get; set; }
    public Guid? SelectedSlotId { get; set; }
    public string? SelectedSlotTime { get; set; }
    public Guid? SelectedAppointmentId { get; set; }
}
