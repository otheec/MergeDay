using System.Net.Http.Headers;
using System.Text;
using MergeDay.Api.Features.Toggl.Connector;
using Refit;

namespace MergeDay.Api.Features.Toggl;

public static class IServiceCollection_Toggl
{
    public static void AddToggl(this IServiceCollection services, string ApiToken, string BaseUrl)
    {
        services.AddRefitClient<ITogglApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                if (string.IsNullOrWhiteSpace(ApiToken))
                    throw new InvalidOperationException("Toggl:ApiToken not configured.");

                var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ApiToken}:api_token"));

                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", authValue);
            });
        services.AddScoped<TogglService>();
    }
}
