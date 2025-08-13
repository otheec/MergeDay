namespace MergeDayApi.Domain.Entities;

public class Workspace
{
    public long Id { get; set; }
    public ICollection<string> UserIds { get; set; } = [];
}
