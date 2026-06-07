using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.BuildingBlocks.Domain;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Entities;

namespace ClinicService.Infrastructure.Persistence;

public sealed class ClinicDbContext(DbContextOptions<ClinicDbContext> options, IMediator mediator)
    : DbContext(options)
{
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<DoctorService> DoctorServices => Set<DoctorService>();
    public DbSet<ScheduleTemplate> ScheduleTemplates => Set<ScheduleTemplate>();
    public DbSet<ScheduleBlock> ScheduleBlocks => Set<ScheduleBlock>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var domainEvents = CollectAndClearDomainEvents();
        var result = await base.SaveChangesAsync(ct);
        foreach (var evt in domainEvents)
            await mediator.Publish(evt, ct);
        return result;
    }

    private List<IDomainEvent> CollectAndClearDomainEvents()
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot<Guid>>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var events = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ForEach(a => a.ClearDomainEvents());
        return events;
    }
}
