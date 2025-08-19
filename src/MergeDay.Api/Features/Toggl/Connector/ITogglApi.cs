using MergeDay.Api.Features.Toggl.Connector.Respose;
using Refit;

namespace MergeDay.Api.Features.Toggl.Connector;

public interface ITogglApi
{
    [Get("/me/time_entries")]
    Task<List<TogglTimeEntryDto>> GetTimeEntriesAsync(
        [AliasAs("start_date")] string startDateIso,
        [AliasAs("end_date")] string endDateIso,
        [AliasAs("meta")] bool includeMeta = true);
}
