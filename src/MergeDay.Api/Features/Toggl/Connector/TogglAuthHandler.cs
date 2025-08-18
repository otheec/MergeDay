using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Features.Toggl.Connector;

public class TogglAuthHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MergeDayDbContext _db;

    public TogglAuthHandler(IHttpContextAccessor httpContextAccessor, MergeDayDbContext db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            throw new InvalidOperationException("No authenticated user.");

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || string.IsNullOrEmpty(user.TogglApiToken))
            throw new InvalidOperationException("User does not have a Toggl API token.");

        var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user.TogglApiToken}:api_token"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        return await base.SendAsync(request, cancellationToken);
    }
}

