using System.Net.Http.Headers;
using System.Text;
using Refit;

namespace MergeDay.Api.Features.Toggl;

public static class IServiceCollection_Toggl
{
    public static void AddToggl(this IServiceCollection services)
    {
        services.AddRefitClient<ITogglApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                var apiToken = config["Toggl:ApiToken"];
                if (string.IsNullOrWhiteSpace(apiToken))
                    throw new InvalidOperationException("Toggl:ApiToken not configured.");

                var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiToken}:api_token"));

                client.BaseAddress = new Uri("https://api.track.toggl.com");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", authValue);
            });
        services.AddScoped<TogglService>();
    }
}
