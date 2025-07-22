using Timescaler.Domain.ValueObjects;

namespace Timescaler.Domain.Entities;

public class Result
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; }
    public DateTime FirstOperationDate { get; private set; }
    public TimeSpan TimeDelta { get; private set; }
    public double AverageExecutionTime { get; private set; }
    public decimal AverageValue { get; private set; }
    public decimal MedianValue { get; private set; }
    public decimal MaxValue { get; private set; }
    public decimal MinValue { get; private set; }

    private readonly List<RawValue> _rawValues = new();
    public IReadOnlyList<RawValue> RawValues => _rawValues.AsReadOnly();

    private Result() { }

    public static Result Create(string fileName, IReadOnlyList<RawDataPoint> records)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Имя файла не может быть пустым.", nameof(fileName));

        if (records == null || !records.Any())
            throw new ArgumentException("Коллекция записей не может быть пустой.", nameof(records));

        var dates = records.Select(r => r.Date).ToArray();
        var values = records.Select(r => r.Value).OrderBy(v => v).ToArray();

        var result = new Result
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            FirstOperationDate = dates.Min(),
            TimeDelta = dates.Max() - dates.Min(),
            AverageExecutionTime = records.Average(r => r.ExecutionTime),
            AverageValue = records.Average(r => r.Value),
            MedianValue = CalculateMedian(values),
            MaxValue = values.Last(),
            MinValue = values.First()
        };

        foreach (var record in records)
        {
            result._rawValues.Add(new RawValue(record.Date, record.ExecutionTime, record.Value, result));
        }

        return result;
    }

    private static decimal CalculateMedian(IReadOnlyList<decimal> sortedValues)
    {
        var count = sortedValues.Count;
        var mid = count / 2;
        return count % 2 == 0
            ? (sortedValues[mid - 1] + sortedValues[mid]) / 2
            : sortedValues[mid];
    }
}
