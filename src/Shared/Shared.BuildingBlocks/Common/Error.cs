namespace Shared.BuildingBlocks.Common;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string resource, object id) =>
        new($"{resource}.NotFound", $"{resource} with id '{id}' was not found.");

    public static Error Conflict(string resource, string description) =>
        new($"{resource}.Conflict", description);

    public static Error Validation(string field, string description) =>
        new($"Validation.{field}", description);

    public static Error Forbidden(string description = "Access denied.") =>
        new("Authorization.Forbidden", description);
}
