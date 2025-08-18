using MergeDay.Api.Common;
using MergeDay.Api.Features.Toggl.Connector;
using Refit;

namespace MergeDay.Api.Features.Toggl;

public static class IServiceCollection_Toggl
{
    public static void AddToggl(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpContextAccessor();

        services.AddTransient<TogglAuthHandler>();

        services.AddRefitClient<ITogglApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<TogglAuthHandler>();

        services.AddScoped<TogglService>();
    }
}

