using MergeDay.Api.Features.Idoklad.IdokladApi;
using MergeDay.Api.Features.Idoklad.IdokladAuth;
using MergeDay.Api.Features.IDoklad.IdokladAuth;
using Refit;

namespace MergeDay.Api.Features.IDoklad;

public static class IServiceCollection_Idoklad
{
    public static void AddIdoklad(this IServiceCollection services)
    {
        services.AddRefitClient<IIdokladAuthApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("https://identity.idoklad.cz/server/v2");
            });

        services.AddRefitClient<IIdokladApi>()
            .ConfigureHttpClient((sp, c) =>
            {
                var svc = sp.GetRequiredService<IdokladService>();
                var token = svc.GetType();
                c.BaseAddress = new Uri("https://api.idoklad.cz/v3");
            })
            .AddHttpMessageHandler<IdokladAuthHandler>();

        services.AddScoped<IdokladAuthHandler>();
        services.AddScoped<IdokladService>();
    }
}
