namespace ClinicService.Application.Services.Queries.GetServices;

public sealed record ServiceDto(
    Guid Id,
    string Name,
    string Category,
    string? Description,
    int DurationMinutes,
    decimal Price,
    bool IsActive
);
