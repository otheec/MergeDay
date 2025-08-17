using MergeDay.Api.Features.Toggl.Connector.Respose;

namespace MergeDay.Api.Features.Toggl.Connector;

public class TogglService(ITogglApi togglApi)
{
    public async Task<List<TogglTimeEntryDto>> GetTimeEntriesAsync(
        DateTime startDate,
        DateTime endDate)
    {
        var startDateIso = startDate.ToString("O");
        var endDateIso = endDate.ToString("O");

        return await togglApi.GetTimeEntriesAsync(startDateIso, endDateIso);
    }
}
