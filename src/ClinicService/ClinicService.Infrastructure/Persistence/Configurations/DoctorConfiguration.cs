using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicService.Domain.Aggregates;

namespace ClinicService.Infrastructure.Persistence.Configurations;

internal sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctors");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.LastName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Specialization).HasMaxLength(200).IsRequired();
        builder.Property(d => d.PhotoUrl).HasMaxLength(500);
        builder.Property(d => d.Bio).HasMaxLength(2000);

        builder.HasMany(d => d.Services)
            .WithOne()
            .HasForeignKey(ds => ds.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.ScheduleTemplates)
            .WithOne()
            .HasForeignKey(st => st.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.ScheduleBlocks)
            .WithOne()
            .HasForeignKey(sb => sb.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.ClinicId);
        builder.HasIndex(d => d.IsActive);
    }
}
