using Microsoft.Extensions.DependencyInjection;
using ClinicService.Domain.Services;

namespace ClinicService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<SlotGeneratorService>();

        return services;
    }
}
