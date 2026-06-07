using System.Net.Http.Json;

namespace TelegramBotService.HttpClients;

public sealed class ClinicApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<ClinicResponse>> GetClinicsAsync(CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<ClinicResponse>>("api/clinics", ct);
        return result ?? [];
    }

    public async Task<IReadOnlyList<DoctorResponse>> GetDoctorsByClinicAsync(
        Guid clinicId, Guid? serviceId = null, CancellationToken ct = default)
    {
        var url = serviceId.HasValue
            ? $"api/doctors?clinicId={clinicId}&serviceId={serviceId}"
            : $"api/doctors?clinicId={clinicId}";
        var result = await http.GetFromJsonAsync<List<DoctorResponse>>(url, ct);
        return result ?? [];
    }

    public async Task<IReadOnlyList<ServiceResponse>> GetServicesAsync(CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<ServiceResponse>>("api/services", ct);
        return result ?? [];
    }

    public async Task<IReadOnlyList<SlotResponse>> GetAvailableSlotsAsync(Guid doctorId, DateOnly date, CancellationToken ct = default)
    {
        var result = await http.GetFromJsonAsync<List<SlotResponse>>(
            $"api/doctors/{doctorId}/slots?date={date:yyyy-MM-dd}", ct);
        return result ?? [];
    }
}
