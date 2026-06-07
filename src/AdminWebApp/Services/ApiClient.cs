using System.Net.Http.Headers;
using System.Net.Http.Json;
using AdminWebApp.Models;

namespace AdminWebApp.Services;

public sealed class ApiClient(IHttpClientFactory httpClientFactory, AuthState authState)
{
    private HttpClient CreateClient()
    {
        var client = httpClientFactory.CreateClient("api");
        if (!string.IsNullOrEmpty(authState.AccessToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authState.AccessToken);
        return client;
    }

    // ── Clinics ──────────────────────────────────────────────────────
    public Task<List<ClinicDto>?> GetClinicsAsync() =>
        CreateClient().GetFromJsonAsync<List<ClinicDto>>("api/clinics");

    public async Task<(bool Success, string? Error)> CreateClinicAsync(CreateClinicRequest request)
    {
        var response = await CreateClient().PostAsJsonAsync("api/clinics", request);
        return response.IsSuccessStatusCode ? (true, null) : (false, await response.Content.ReadAsStringAsync());
    }

    // ── Doctors ──────────────────────────────────────────────────────
    public Task<List<DoctorDto>?> GetDoctorsAsync(Guid? clinicId = null)
    {
        var url = clinicId.HasValue ? $"api/doctors?clinicId={clinicId}" : "api/doctors";
        return CreateClient().GetFromJsonAsync<List<DoctorDto>>(url);
    }

    public async Task<(bool Success, string? Error)> CreateDoctorAsync(CreateDoctorRequest request)
    {
        var payload = new
        {
            request.ClinicId,
            request.FirstName,
            request.LastName,
            request.Specialization,
            PhotoUrl = (string?)null,
            request.Bio
        };
        var response = await CreateClient().PostAsJsonAsync("api/doctors", payload);
        return response.IsSuccessStatusCode ? (true, null) : (false, await response.Content.ReadAsStringAsync());
    }

    // ── Services ─────────────────────────────────────────────────────
    public Task<List<ServiceDto>?> GetServicesAsync() =>
        CreateClient().GetFromJsonAsync<List<ServiceDto>>("api/services");

    public async Task<(bool Success, string? Error)> CreateServiceAsync(CreateServiceRequest request)
    {
        var response = await CreateClient().PostAsJsonAsync("api/services", request);
        return response.IsSuccessStatusCode ? (true, null) : (false, await response.Content.ReadAsStringAsync());
    }

    // ── Appointments ─────────────────────────────────────────────────
    public Task<List<AppointmentDto>?> GetAllAppointmentsAsync(Guid? clinicId = null, DateOnly? date = null)
    {
        var query = new List<string>();
        if (clinicId.HasValue) query.Add($"clinicId={clinicId}");
        if (date.HasValue) query.Add($"date={date.Value:yyyy-MM-dd}");
        var url = "api/appointments/all" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
        return CreateClient().GetFromJsonAsync<List<AppointmentDto>>(url);
    }

    public async Task<bool> CancelAppointmentAsync(Guid id, Guid patientId)
    {
        var response = await CreateClient().PutAsync($"api/appointments/{id}/cancel?patientId={patientId}", null);
        return response.IsSuccessStatusCode;
    }
}
