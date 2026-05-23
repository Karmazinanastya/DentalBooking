using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.BuildingBlocks.Domain;
using PatientService.Domain.Aggregates;

namespace PatientService.Infrastructure.Persistence;

public sealed class PatientDbContext(DbContextOptions<PatientDbContext> options, IMediator mediator)
    : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var result = await base.SaveChangesAsync(ct);
        await DispatchDomainEventsAsync(ct);
        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot<Guid>>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
                await mediator.Publish(domainEvent, ct);
            aggregate.ClearDomainEvents();
        }
    }
}
