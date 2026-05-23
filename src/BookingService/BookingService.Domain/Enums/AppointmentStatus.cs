namespace BookingService.Domain.Enums;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Completed,
    CancelledByPatient,
    CancelledByClinic,
    Expired
}
