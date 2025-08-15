namespace MergeDay.Api.Domain.Entities;

public class Absence
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AbsenceKind Kind { get; set; }
    public AbsenceStatus Status { get; set; }

    public string RequestorNote { get; set; } = string.Empty;
    public string? AuditorNote { get; set; } = null;
}
