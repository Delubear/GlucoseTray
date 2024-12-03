using GlucoseTray.GlucoseSettings.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GlucoseTray.GlucoseSettings;

public static class DependencyExtensions
{
    public static IServiceCollection RegisterGlucoseSettingsServices(this IServiceCollection services)
    {
        services
                .AddScoped<ISettingsProxy, SettingsProxy>()
                .AddScoped<ISettingsService, SettingsService>()
                .AddScoped<ILocalFileAdapter<GlucoseTraySettings>, FileService<GlucoseTraySettings>>()
                .AddScoped<ISettingsWindowService, SettingsWindowService>();

        return services;
    }
}
