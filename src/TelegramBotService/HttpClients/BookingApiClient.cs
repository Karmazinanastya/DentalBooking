using System.Net.Http.Json;

namespace TelegramBotService.HttpClients;

public sealed class BookingApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<AppointmentResponse>> GetMyAppointmentsAsync(Guid patientId, CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<AppointmentResponse>>(
            $"api/appointments/my?patientId={patientId}", ct);
        return result ?? [];
    }

    public async Task<Guid> CreateAppointmentAsync(CreateAppointmentRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/appointments", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>(ct);
    }

    public async Task ConfirmAsync(Guid appointmentId, Guid patientId, CancellationToken ct = default)
    {
        var response = await http.PostAsync(
            $"api/appointments/{appointmentId}/confirm?patientId={patientId}", null, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task CancelByPatientAsync(Guid appointmentId, Guid patientId, CancellationToken ct = default)
    {
        var response = await http.PutAsync(
            $"api/appointments/{appointmentId}/cancel?patientId={patientId}", null, ct);
        response.EnsureSuccessStatusCode();
    }
}
