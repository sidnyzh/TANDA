namespace Tanda.Domain.Entities;

public sealed class DailyCapacity
{
    public DateOnly Date { get; set; }

    public int TotalEsfuerzo { get; set; }

    public int CommittedEsfuerzo { get; set; }
}
