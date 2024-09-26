using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.FetchResults;
using Microsoft.Extensions.DependencyInjection;

namespace GlucoseTray.Domain;

public static class DependencyExtensions
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IRunner, Runner>()
                .AddScoped<DebugService, DebugService>()
                .AddScoped<AlertService, AlertService>()
                .AddScoped<INightscoutService, NightscoutService>()
                .AddScoped<IDexcomService, DexcomService>()
                .AddScoped<UrlAssembler, UrlAssembler>()
                .AddScoped<IGlucoseFetchService, GlucoseFetchService>();

        return services;
    }
}
