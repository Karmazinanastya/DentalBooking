using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdminWebApp.Models;

namespace AdminWebApp.Services;

public sealed class ApiClient(IHttpClientFactory httpClientFactory, AuthState authState)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private HttpClient CreateClient()
    {
        var client = httpClientFactory.CreateClient("api");
        if (!string.IsNullOrEmpty(authState.AccessToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authState.AccessToken);
        return client;
    }

    // ── Clinics ──────────────────────────────────────────────────────
    public Task<List<ClinicDto>?> GetClinicsAsync() =>
        CreateClient().GetFromJsonAsync<List<ClinicDto>>("api/clinics", JsonOptions);

    public async Task<(bool Success, string? Error)> CreateClinicAsync(CreateClinicRequest request)
    {
        var response = await CreateClient().PostAsJsonAsync("api/clinics", request, JsonOptions);
        return response.IsSuccessStatusCode ? (true, null) : (false, await response.Content.ReadAsStringAsync());
    }

    // ── Doctors ──────────────────────────────────────────────────────
    public Task<List<DoctorDto>?> GetDoctorsAsync(Guid? clinicId = null)
    {
        var url = clinicId.HasValue ? $"api/doctors?clinicId={clinicId}" : "api/doctors";
        return CreateClient().GetFromJsonAsync<List<DoctorDto>>(url, JsonOptions);
    }

    public async Task<(bool Success, Guid? DoctorId, string? Error)> CreateDoctorAsync(CreateDoctorRequest request)
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
        var response = await CreateClient().PostAsJsonAsync("api/doctors", payload, JsonOptions);
        if (!response.IsSuccessStatusCode)
            return (false, null, await response.Content.ReadAsStringAsync());
        var doctorId = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        return (true, doctorId, null);
    }

    // ── Schedule & Slots ─────────────────────────────────────────────
    public Task<DoctorScheduleDto?> GetDoctorScheduleAsync(Guid doctorId) =>
        CreateClient().GetFromJsonAsync<DoctorScheduleDto>($"api/doctors/{doctorId}/schedule", JsonOptions);

    public async Task<(bool Success, string? Error)> SetDoctorScheduleAsync(
        Guid doctorId, DayOfWeek day, string workStart, string workEnd,
        string? lunchStart, string? lunchEnd)
    {
        var payload = new
        {
            DoctorId = doctorId,
            DayOfWeek = day,
            WorkStart = TimeOnly.Parse(workStart),
            WorkEnd = TimeOnly.Parse(workEnd),
            LunchStart = string.IsNullOrEmpty(lunchStart) ? (TimeOnly?)null : TimeOnly.Parse(lunchStart),
            LunchEnd = string.IsNullOrEmpty(lunchEnd) ? (TimeOnly?)null : TimeOnly.Parse(lunchEnd)
        };
        var r = await CreateClient().PutAsJsonAsync($"api/doctors/{doctorId}/schedule", payload, JsonOptions);
        return r.IsSuccessStatusCode ? (true, null) : (false, await r.Content.ReadAsStringAsync());
    }

    public async Task<(bool Success, int Count, string? Error)> GenerateSlotsAsync(
        Guid doctorId, DateOnly from, DateOnly to)
    {
        var r = await CreateClient().PostAsJsonAsync(
            $"api/doctors/{doctorId}/slots/generate",
            new GenerateSlotsRequest(from, to),
            JsonOptions);
        if (r.IsSuccessStatusCode)
        {
            var count = await r.Content.ReadFromJsonAsync<int>(JsonOptions);
            return (true, count, null);
        }
        return (false, 0, await r.Content.ReadAsStringAsync());
    }

    // ── Services ─────────────────────────────────────────────────────
    public Task<List<ServiceDto>?> GetServicesAsync() =>
        CreateClient().GetFromJsonAsync<List<ServiceDto>>("api/services", JsonOptions);

    public async Task<(bool Success, string? Error)> CreateServiceAsync(CreateServiceRequest request)
    {
        var response = await CreateClient().PostAsJsonAsync("api/services", request, JsonOptions);
        return response.IsSuccessStatusCode ? (true, null) : (false, await response.Content.ReadAsStringAsync());
    }

    // ── Appointments ─────────────────────────────────────────────────
    public Task<List<AppointmentDto>?> GetAllAppointmentsAsync(
        Guid? clinicId = null, DateOnly? date = null, Guid? doctorId = null)
    {
        var query = new List<string>();
        if (clinicId.HasValue) query.Add($"clinicId={clinicId}");
        if (doctorId.HasValue) query.Add($"doctorId={doctorId}");
        if (date.HasValue) query.Add($"date={date.Value:yyyy-MM-dd}");
        var url = "api/appointments/all" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
        return CreateClient().GetFromJsonAsync<List<AppointmentDto>>(url, JsonOptions);
    }

    public async Task<bool> CancelAppointmentAsync(Guid id, Guid patientId)
    {
        var response = await CreateClient().PutAsync($"api/appointments/{id}/cancel?patientId={patientId}", null);
        return response.IsSuccessStatusCode;
    }
}
