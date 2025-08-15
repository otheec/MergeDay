using Refit;

namespace MergeDay.Api.Features.Toggl;

public interface ITogglApi
{
    [Get("/api/v9/me/time_entries")]
    Task<List<TogglTimeEntryDto>> GetTimeEntriesAsync(
        [AliasAs("start_date")] string startDateIso,
        [AliasAs("end_date")] string endDateIso,
        [AliasAs("meta")] bool includeMeta = true);
}
