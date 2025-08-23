namespace MergeDay.Api.Domain.Entities;

public class Workspace
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> UserIds { get; set; } = [];
}
