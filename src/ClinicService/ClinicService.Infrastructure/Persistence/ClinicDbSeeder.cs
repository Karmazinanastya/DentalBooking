using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ClinicService.Application.Clinics.Commands.CreateClinic;
using ClinicService.Application.Doctors.Commands.CreateDoctor;
using ClinicService.Domain.Entities;
using ClinicService.Domain.ValueObjects;

namespace ClinicService.Infrastructure.Persistence;

public static class ClinicDbSeeder
{
    public static async Task SeedAsync(IServiceProvider scopedServices)
    {
        var db = scopedServices.GetRequiredService<ClinicDbContext>();
        var mediator = scopedServices.GetRequiredService<IMediator>();

        if (await db.Clinics.AnyAsync())
            return;

        // ── Services ──────────────────────────────────────────────
        var sConsultation = Service.Create("Консультація стоматолога", "Діагностика", 30, 500m,
            "Первинний огляд, діагностика, складання плану лікування");
        var sCleaning = Service.Create("Професійна чистка зубів", "Гігієна", 60, 800m,
            "Ультразвукова чистка + Air Flow + полірування");
        var sFilling = Service.Create("Лікування карієсу", "Терапія", 45, 1200m,
            "Лікування карієсу з установкою фотополімерної пломби");

        await db.Services.AddRangeAsync(sConsultation, sCleaning, sFilling);
        await db.SaveChangesAsync(CancellationToken.None);

        // ── Clinics ───────────────────────────────────────────────
        const string timeZone = "Europe/Kyiv";

        var clinic1 = (await mediator.Send(new CreateClinicCommand(
            "DentalBook — Центральна",
            "Київ", "вул. Хрещатик", "10",
            "+380441234567", timeZone,
            "Головна клініка мережі DentalBook у центрі міста", null))).Value;

        var clinic2 = (await mediator.Send(new CreateClinicCommand(
            "DentalBook — Позняки",
            "Київ", "вул. Ревуцького", "5",
            "+380442345678", timeZone,
            "Філія DentalBook на Позняках", null))).Value;

        // ── Doctors ───────────────────────────────────────────────
        var doctor1 = (await mediator.Send(new CreateDoctorCommand(
            clinic1, "Олена", "Ковальчук", "Терапевт", null,
            "10 років досвіду. Терапевтична стоматологія та естетична реставрація."))).Value;

        var doctor2 = (await mediator.Send(new CreateDoctorCommand(
            clinic1, "Максим", "Бондаренко", "Хірург", null,
            "Кандидат медичних наук. Імплантація та хірургічні втручання."))).Value;

        var doctor3 = (await mediator.Send(new CreateDoctorCommand(
            clinic2, "Аліна", "Петренко", "Гігієніст", null,
            "Профілактична стоматологія та гігієна ротової порожнини."))).Value;

        // ── Assign services to doctors ────────────────────────────
        var d1 = await db.Doctors.FindAsync(doctor1);
        d1!.AddService(sConsultation.Id, 30);
        d1.AddService(sCleaning.Id, 60);
        d1.AddService(sFilling.Id, 45);

        var d2 = await db.Doctors.FindAsync(doctor2);
        d2!.AddService(sConsultation.Id, 30);

        var d3 = await db.Doctors.FindAsync(doctor3);
        d3!.AddService(sConsultation.Id, 30);
        d3.AddService(sCleaning.Id, 60);

        // ── Schedule templates ────────────────────────────────────
        var work = WorkingHours.Create(new TimeOnly(9, 0), new TimeOnly(18, 0)).Value;
        var lunch = WorkingHours.Create(new TimeOnly(13, 0), new TimeOnly(14, 0)).Value;
        var weekendWork = WorkingHours.Create(new TimeOnly(10, 0), new TimeOnly(15, 0)).Value;

        // Ковальчук і Петренко: Пн–Пт 09:00–18:00 + Сб–Нд 10:00–15:00 (без обіду)
        foreach (var day in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                                    DayOfWeek.Thursday, DayOfWeek.Friday })
        {
            d1.SetScheduleTemplate(day, work, lunch);
            d3.SetScheduleTemplate(day, work, lunch);
        }

        d1.SetScheduleTemplate(DayOfWeek.Saturday, weekendWork, null);
        d1.SetScheduleTemplate(DayOfWeek.Sunday, weekendWork, null);
        d3.SetScheduleTemplate(DayOfWeek.Saturday, weekendWork, null);

        // Бондаренко: тільки Пн–Пт 09:00–18:00
        foreach (var day in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                                    DayOfWeek.Thursday, DayOfWeek.Friday })
        {
            d2.SetScheduleTemplate(day, work, lunch);
        }

        // SaveChangesAsync fires DoctorScheduleUpdatedEvent → GenerateSlotsCommand (30 days ahead)
        await db.SaveChangesAsync(CancellationToken.None);
    }
}
