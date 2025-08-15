using System.Net.Http.Headers;
using System.Text;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using MergeDay.Api.Infrastructure.TogglConnector;
using Microsoft.EntityFrameworkCore;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddDbContext<MergeDayDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MergeDayDb")));
builder.Services.AddRefitClient<ITogglApi>()
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
builder.Services.AddScoped<TogglService>();

var app = builder.Build();

app.MapEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
