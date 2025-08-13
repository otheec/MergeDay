namespace MergeDayApi.Domain.Entities;

public class Workspace
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<string> UserIds { get; set; } = [];
}
