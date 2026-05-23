using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookingService.Domain.Aggregates;

namespace BookingService.Infrastructure.Persistence.Configurations;

internal sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.PatientId).IsRequired();
        builder.Property(a => a.PatientChatId).IsRequired();
        builder.Property(a => a.SlotId).IsRequired();
        builder.Property(a => a.DoctorId).IsRequired();
        builder.Property(a => a.DoctorFullName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.ClinicId).IsRequired();
        builder.Property(a => a.ClinicName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.ClinicAddress).HasMaxLength(400).IsRequired();
        builder.Property(a => a.ClinicTimeZoneId).HasMaxLength(100).IsRequired();
        builder.Property(a => a.ServiceName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.AppointmentDateUtc).IsRequired();
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(a => a.CancellationReason).HasMaxLength(500);

        builder.HasIndex(a => a.PatientId);
        builder.HasIndex(a => a.ClinicId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => new { a.PatientId, a.AppointmentDateUtc });
    }
}
