using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicService.Domain.Entities;

namespace ClinicService.Infrastructure.Persistence.Configurations;

internal sealed class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("time_slots");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(t => t.StartUtc).IsRequired();
        builder.Property(t => t.EndUtc).IsRequired();

        builder.HasIndex(t => new { t.DoctorId, t.StartUtc }).IsUnique();
        builder.HasIndex(t => new { t.DoctorId, t.Status });
        builder.HasIndex(t => t.Status);

    }
}
