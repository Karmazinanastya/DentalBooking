using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ClinicService.Application.Clinics.Commands.CreateClinic;
using ClinicService.Application.Doctors.Commands.CreateDoctor;
using ClinicService.Application.Slots.Commands.GenerateSlots;
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

        // ── DoctorServices (direct insert, bypassing aggregate tracking) ──
        await db.DoctorServices.AddRangeAsync(
            DoctorService.Create(doctor1, sConsultation.Id, 30),
            DoctorService.Create(doctor1, sCleaning.Id, 60),
            DoctorService.Create(doctor1, sFilling.Id, 45),
            DoctorService.Create(doctor2, sConsultation.Id, 30),
            DoctorService.Create(doctor3, sConsultation.Id, 30),
            DoctorService.Create(doctor3, sCleaning.Id, 60));

        // ── Schedule templates (direct insert, bypassing aggregate tracking) ──
        var work = WorkingHours.Create(new TimeOnly(9, 0), new TimeOnly(18, 0)).Value;
        var lunch = WorkingHours.Create(new TimeOnly(13, 0), new TimeOnly(14, 0)).Value;
        var weekendWork = WorkingHours.Create(new TimeOnly(10, 0), new TimeOnly(15, 0)).Value;

        var weekdays = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                               DayOfWeek.Thursday, DayOfWeek.Friday };

        // Ковальчук: Пн–Пт 09:00–18:00 + Сб–Нд 10:00–15:00
        foreach (var day in weekdays)
            await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(doctor1, clinic1, day, work, lunch));
        await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(doctor1, clinic1, DayOfWeek.Saturday, weekendWork, null));
        await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(doctor1, clinic1, DayOfWeek.Sunday, weekendWork, null));

        // Бондаренко: тільки Пн–Пт 09:00–18:00
        foreach (var day in weekdays)
            await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(doctor2, clinic1, day, work, lunch));

        // Петренко: Пн–Пт 09:00–18:00 + Сб 10:00–15:00
        foreach (var day in weekdays)
            await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(doctor3, clinic2, day, work, lunch));
        await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(doctor3, clinic2, DayOfWeek.Saturday, weekendWork, null));

        await db.SaveChangesAsync(CancellationToken.None);

        // ── Generate slots for the next 30 days ──────────────────
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var until = today.AddDays(30);
        await mediator.Send(new GenerateSlotsCommand(doctor1, today, until), CancellationToken.None);
        await mediator.Send(new GenerateSlotsCommand(doctor2, today, until), CancellationToken.None);
        await mediator.Send(new GenerateSlotsCommand(doctor3, today, until), CancellationToken.None);
    }
}
