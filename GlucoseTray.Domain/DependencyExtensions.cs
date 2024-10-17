using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.FetchResults;
using GlucoseTray.Domain.FetchResults.Dexcom;
using GlucoseTray.Domain.FetchResults.Nightscout;
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
                .AddScoped<GlucoseResult, GlucoseResult>()
                .AddScoped<IGlucoseFetchService, GlucoseFetchService>();

        return services;
    }
}
