using GlucoseTray.DisplayResults.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GlucoseTray.DisplayResults;

public static class DependencyExtensions
{
    public static IServiceCollection RegisterDisplayResultServices(this IServiceCollection services)
    {
        services
                .AddScoped<AlertService, AlertService>()
                .AddScoped<ISchedulingAdapter, TaskSchedulerService>()
                .AddScoped<IIconService, IconService>()
                .AddScoped<IDialogService, DialogService>();

        return services;
    }
}
