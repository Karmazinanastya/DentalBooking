using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using NotificationService.Application.Abstractions;
using NotificationService.Infrastructure.Consumers;
using NotificationService.Infrastructure.Scheduling;
using NotificationService.Infrastructure.Telegram;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ITelegramBotClient>(
            new TelegramBotClient(configuration["Telegram:BotToken"]!));

        services.AddScoped<ITelegramSender, TelegramSender>();
        services.AddSingleton<IScheduler, HangfireScheduler>();
        services.AddScoped<ReminderJob>();

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(opts =>
                opts.UseNpgsqlConnection(configuration.GetConnectionString("Default")!)));

        services.AddHangfireServer();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<PatientRegisteredConsumer>();
            x.AddConsumer<AppointmentCreatedConsumer>();
            x.AddConsumer<AppointmentCancelledByPatientConsumer>();
            x.AddConsumer<AppointmentCancelledByClinicConsumer>();
            x.AddConsumer<AppointmentCompletedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], h =>
                {
                    h.Username(configuration["RabbitMQ:User"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
