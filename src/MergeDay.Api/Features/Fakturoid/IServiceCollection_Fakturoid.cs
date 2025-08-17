using MergeDay.Api.Common;
using MergeDay.Api.Features.Fakturoid.Auth;
using MergeDay.Api.Features.Fakturoid.Connector;
using Refit;

namespace MergeDay.Api.Features.Fakturoid;

public static class IServiceCollection_Fakturoid
{
    public static void AddFakturoid(this IServiceCollection services, string baseUrl)
    {
        services.AddOptions<FakturoidOptions>()
            .BindConfiguration("Fakturoid")
            .ValidateDataAnnotations();

        services.AddRefitClient<IFakturoidAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        services.AddScoped<FakturoidAuthService>();
        services.AddTransient<FakturoidAuthHandler>();

        services.AddRefitClient<IFakturoidApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<FakturoidAuthHandler>();

        services.AddScoped<FakturoidService>();
    }
}

