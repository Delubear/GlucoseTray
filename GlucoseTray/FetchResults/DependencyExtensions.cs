using GlucoseTray.FetchResults.Contracts;
using GlucoseTray.FetchResults.Dexcom;
using GlucoseTray.FetchResults.Nightscout;
using Microsoft.Extensions.DependencyInjection;

namespace GlucoseTray.FetchResults;

public static class DependencyExtensions
{
    public static IServiceCollection RegisterFetchResultsServices(this IServiceCollection services)
    {
        services
                .AddScoped<DebugService, DebugService>()
                .AddScoped<INightscoutService, NightscoutService>()
                .AddScoped<IDexcomService, DexcomService>()
                .AddScoped<UrlAssembler, UrlAssembler>()
                .AddScoped<IGlucoseFetchService, GlucoseFetchService>()
                .AddScoped<IExternalCommunicationAdapter, ExternalCommunicationAdapter>();

        return services;
    }
}
