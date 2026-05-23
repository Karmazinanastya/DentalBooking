using System.Net;
using System.Net.Http.Json;

namespace TelegramBotService.HttpClients;

public sealed class PatientApiClient(HttpClient http)
{
    public async Task<PatientResponse?> GetByChatIdAsync(long chatId, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"api/patients/by-telegram/{chatId}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PatientResponse>(ct);
    }

    public async Task<Guid> RegisterAsync(RegisterPatientRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/patients/register", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>(ct);
    }
}
