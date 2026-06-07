using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicService.Domain.Entities;

namespace ClinicService.Infrastructure.Persistence.Configurations;

internal sealed class ScheduleTemplateConfiguration : IEntityTypeConfiguration<ScheduleTemplate>
{
    public void Configure(EntityTypeBuilder<ScheduleTemplate> builder)
    {
        builder.ToTable("schedule_templates");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.DayOfWeek).IsRequired();
        builder.Property(s => s.WorkStart).HasColumnName("work_start").IsRequired();
        builder.Property(s => s.WorkEnd).HasColumnName("work_end").IsRequired();
        builder.Property(s => s.LunchStart).HasColumnName("lunch_start");
        builder.Property(s => s.LunchEnd).HasColumnName("lunch_end");

        builder.Ignore(s => s.WorkingHours);
        builder.Ignore(s => s.LunchBreak);

        builder.HasIndex(s => new { s.DoctorId, s.DayOfWeek }).IsUnique();
    }
}
