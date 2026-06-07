using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ClinicService.Application.Clinics.Commands.CreateClinic;
using ClinicService.Application.Slots.Commands.GenerateSlots;
using ClinicService.Domain.Aggregates;
using ClinicService.Domain.Entities;
using ClinicService.Domain.ValueObjects;

namespace ClinicService.Infrastructure.Persistence;

public static class ClinicDbSeeder
{
    // Fixed GUIDs so IdentityService seeder can reference the same DoctorId values
    public static readonly Guid Doctor1Id = new("d0000001-0000-0000-0000-000000000001"); // Ковальчук
    public static readonly Guid Doctor2Id = new("d0000002-0000-0000-0000-000000000001"); // Бондаренко
    public static readonly Guid Doctor3Id = new("d0000003-0000-0000-0000-000000000001"); // Петренко

    public static async Task SeedAsync(IServiceProvider scopedServices)
    {
        var db = scopedServices.GetRequiredService<ClinicDbContext>();
        var mediator = scopedServices.GetRequiredService<IMediator>();

        if (await db.Clinics.AnyAsync())
            return;

        // ── Services ──────────────────────────────────────────────
        var sConsultation = Service.Create("Консультація стоматолога", "Діагностика", 60, 500m,
            "Первинний огляд, діагностика, складання плану лікування");
        var sCleaning = Service.Create("Професійна чистка зубів", "Гігієна", 60, 800m,
            "Ультразвукова чистка + Air Flow + полірування");
        var sFilling = Service.Create("Лікування карієсу", "Терапія", 60, 1200m,
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

        // ── Doctors (fixed GUIDs for cross-service linking) ───────
        var doc1 = Doctor.Create(clinic1, "Олена", "Ковальчук", "Терапевт", null,
            "10 років досвіду. Терапевтична стоматологія та естетична реставрація.",
            seedId: Doctor1Id);

        var doc2 = Doctor.Create(clinic1, "Максим", "Бондаренко", "Хірург", null,
            "Кандидат медичних наук. Імплантація та хірургічні втручання.",
            seedId: Doctor2Id);

        var doc3 = Doctor.Create(clinic2, "Аліна", "Петренко", "Гігієніст", null,
            "Профілактична стоматологія та гігієна ротової порожнини.",
            seedId: Doctor3Id);

        await db.Doctors.AddRangeAsync(doc1, doc2, doc3);

        // ── DoctorServices ────────────────────────────────────────
        await db.DoctorServices.AddRangeAsync(
            DoctorService.Create(Doctor1Id, sConsultation.Id, 60),
            DoctorService.Create(Doctor1Id, sCleaning.Id, 60),
            DoctorService.Create(Doctor1Id, sFilling.Id, 60),
            DoctorService.Create(Doctor2Id, sConsultation.Id, 60),
            DoctorService.Create(Doctor2Id, sFilling.Id, 60),
            DoctorService.Create(Doctor3Id, sConsultation.Id, 60),
            DoctorService.Create(Doctor3Id, sCleaning.Id, 60));

        // ── Schedule templates ────────────────────────────────────
        var work        = WorkingHours.Create(new TimeOnly(9, 0), new TimeOnly(18, 0)).Value;
        var lunch       = WorkingHours.Create(new TimeOnly(13, 0), new TimeOnly(14, 0)).Value;
        var weekendWork = WorkingHours.Create(new TimeOnly(10, 0), new TimeOnly(15, 0)).Value;

        var weekdays = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                               DayOfWeek.Thursday, DayOfWeek.Friday };

        // Ковальчук: Пн–Пт 09:00–18:00 (обід 13–14) + Сб 10:00–15:00
        foreach (var day in weekdays)
            await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(Doctor1Id, clinic1, day, work, lunch));
        await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(Doctor1Id, clinic1, DayOfWeek.Saturday, weekendWork, null));

        // Бондаренко: Пн–Пт 09:00–18:00 (обід 13–14)
        foreach (var day in weekdays)
            await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(Doctor2Id, clinic1, day, work, lunch));

        // Петренко: Пн–Пт 09:00–18:00 (обід 13–14) + Сб 10:00–15:00
        foreach (var day in weekdays)
            await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(Doctor3Id, clinic2, day, work, lunch));
        await db.ScheduleTemplates.AddAsync(ScheduleTemplate.Create(Doctor3Id, clinic2, DayOfWeek.Saturday, weekendWork, null));

        await db.SaveChangesAsync(CancellationToken.None);

        // ── Generate slots for the next 30 days ──────────────────
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var until = today.AddDays(30);
        await mediator.Send(new GenerateSlotsCommand(Doctor1Id, today, until), CancellationToken.None);
        await mediator.Send(new GenerateSlotsCommand(Doctor2Id, today, until), CancellationToken.None);
        await mediator.Send(new GenerateSlotsCommand(Doctor3Id, today, until), CancellationToken.None);
    }
}
