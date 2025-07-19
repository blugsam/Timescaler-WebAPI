namespace Timescaler.Domain.Entities;

public class ValueEntry
{
    public Guid Id { get; set; }
    public string SourceFileName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public decimal Value { get; set; }

    public Guid ResultEntryId { get; set; }
    public ResultEntry ResultEntry { get; set; } = null!;
}