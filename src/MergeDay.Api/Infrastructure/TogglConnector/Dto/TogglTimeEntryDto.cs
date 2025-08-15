using System.Text.Json.Serialization;

namespace MergeDay.Api.Infrastructure.TogglConnector.Dto;

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

    [JsonPropertyName("project_name")]
    public string? ProjectName { get; set; }

    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }

    [JsonPropertyName("task_name")]
    public string? TaskName { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];
}

