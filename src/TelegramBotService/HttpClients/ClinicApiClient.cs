using System.Net.Http.Json;

namespace TelegramBotService.HttpClients;

public sealed class ClinicApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<ClinicResponse>> GetClinicsAsync(CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<ClinicResponse>>("api/clinics", ct);
        return result ?? [];
    }

    public async Task<IReadOnlyList<DoctorResponse>> GetDoctorsByClinicAsync(Guid clinicId, CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<DoctorResponse>>($"api/doctors?clinicId={clinicId}", ct);
        return result ?? [];
    }

    public async Task<IReadOnlyList<SlotResponse>> GetAvailableSlotsAsync(Guid doctorId, DateOnly date, CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<SlotResponse>>(
            $"api/doctors/{doctorId}/slots?date={date:yyyy-MM-dd}", ct);
        return result ?? [];
    }
}
