using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Domain;

namespace ClinicService.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public string City { get; }
    public string Street { get; }
    public string BuildingNumber { get; }

    private Address(string city, string street, string buildingNumber)
    {
        City = city;
        Street = street;
        BuildingNumber = buildingNumber;
    }

    public static Result<Address> Create(string city, string street, string buildingNumber)
    {
        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure<Address>(Error.Validation(nameof(City), "City is required."));
        if (string.IsNullOrWhiteSpace(street))
            return Result.Failure<Address>(Error.Validation(nameof(Street), "Street is required."));
        if (string.IsNullOrWhiteSpace(buildingNumber))
            return Result.Failure<Address>(Error.Validation(nameof(BuildingNumber), "Building number is required."));

        return new Address(city.Trim(), street.Trim(), buildingNumber.Trim());
    }

    public override string ToString() => $"{City}, {Street} {BuildingNumber}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return City.ToLowerInvariant();
        yield return Street.ToLowerInvariant();
        yield return BuildingNumber.ToLowerInvariant();
    }
}
