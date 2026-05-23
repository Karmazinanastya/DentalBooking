using Shared.BuildingBlocks.Domain;

namespace ClinicService.Domain.Entities;

public sealed class Service : Entity<Guid>
{
    public string Name { get; private set; }
    public string Category { get; private set; }
    public string? Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }

    private Service() { }

    public static Service Create(
        string name,
        string category,
        int durationMinutes,
        decimal price,
        string? description = null)
    {
        return new Service
        {
            Id = Guid.NewGuid(),
            Name = name,
            Category = category,
            DurationMinutes = durationMinutes,
            Price = price,
            Description = description,
            IsActive = true
        };
    }

    public void Update(string name, string category, int durationMinutes, decimal price, string? description)
    {
        Name = name;
        Category = category;
        DurationMinutes = durationMinutes;
        Price = price;
        Description = description;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
