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

        builder.OwnsOne(s => s.WorkingHours, wh =>
        {
            wh.Property(x => x.Start).HasColumnName("work_start").IsRequired();
            wh.Property(x => x.End).HasColumnName("work_end").IsRequired();
        });

        builder.OwnsOne(s => s.LunchBreak, lb =>
        {
            lb.Property(x => x.Start).HasColumnName("lunch_start");
            lb.Property(x => x.End).HasColumnName("lunch_end");
        });

        builder.HasIndex(s => new { s.DoctorId, s.DayOfWeek }).IsUnique();
    }
}
