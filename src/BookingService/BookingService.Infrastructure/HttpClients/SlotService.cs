using System.Net.Http.Json;
using Shared.BuildingBlocks.Common;
using BookingService.Application.Abstractions;

namespace BookingService.Infrastructure.HttpClients;

internal sealed class SlotService(HttpClient httpClient) : ISlotService
{
    public async Task<Result<SlotInfo>> GetSlotInfoAsync(Guid slotId, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"slots/{slotId}", ct);
        if (!response.IsSuccessStatusCode)
            return Result.Failure<SlotInfo>(Error.NotFound("Slot", slotId));

        var dto = await response.Content.ReadFromJsonAsync<SlotInfoDto>(ct);
        if (dto is null)
            return Result.Failure<SlotInfo>(Error.NotFound("Slot", slotId));

        return new SlotInfo(
            dto.SlotId, dto.DoctorId, dto.DoctorFullName,
            dto.ClinicId, dto.ClinicName, dto.ClinicAddress,
            dto.ClinicTimeZoneId, dto.ServiceName, dto.StartUtc);
    }

    public async Task<Result> ReserveSlotAsync(Guid slotId, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsync($"slots/{slotId}/reserve", null, ct);
        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(Error.Conflict("Slot", "Slot is no longer available."));
    }

    public async Task<Result> BookSlotAsync(Guid slotId, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsync($"slots/{slotId}/book", null, ct);
        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(Error.Conflict("Slot", "Failed to book slot."));
    }

    public async Task<Result> ReleaseSlotAsync(Guid slotId, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsync($"slots/{slotId}/release", null, ct);
        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(Error.Conflict("Slot", "Failed to release slot."));
    }

    private sealed record SlotInfoDto(
        Guid SlotId,
        Guid DoctorId,
        string DoctorFullName,
        Guid ClinicId,
        string ClinicName,
        string ClinicAddress,
        string ClinicTimeZoneId,
        string ServiceName,
        DateTime StartUtc
    );
}
