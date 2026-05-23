using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicService.Domain.Aggregates;

namespace ClinicService.Infrastructure.Persistence.Configurations;

internal sealed class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.ToTable("clinics");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Phone).HasMaxLength(30).IsRequired();
        builder.Property(c => c.TimeZoneId).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.PhotoUrl).HasMaxLength(500);

        builder.OwnsOne(c => c.Address, a =>
        {
            a.Property(x => x.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            a.Property(x => x.Street).HasColumnName("street").HasMaxLength(200).IsRequired();
            a.Property(x => x.BuildingNumber).HasColumnName("building_number").HasMaxLength(20).IsRequired();
        });

        builder.HasIndex(c => c.IsActive);
    }
}
