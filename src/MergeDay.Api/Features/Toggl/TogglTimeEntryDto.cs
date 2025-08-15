using System.Text.Json.Serialization;

namespace MergeDay.Api.Features.Toggl;

public class TogglTimeEntryDto
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    [JsonPropertyName("stop")]
    public DateTime? Stop { get; set; }

    [JsonPropertyName("project_id")]
    public long? ProjectId { get; set; }

    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }
}