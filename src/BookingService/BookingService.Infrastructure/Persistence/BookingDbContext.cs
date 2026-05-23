using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.BuildingBlocks.Domain;
using BookingService.Domain.Aggregates;

namespace BookingService.Infrastructure.Persistence;

public sealed class BookingDbContext(DbContextOptions<BookingDbContext> options, IMediator mediator)
    : DbContext(options)
{
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
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
