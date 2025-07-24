using Timescaler.Domain.Entities;
using Timescaler.Domain.ValueObjects;

namespace Timescaler.Tests;

public class ResultTests
{
    [Fact]
    public void Create_WithValidData_ShouldCalculateAggregatesCorrectly()
    {
        var fileName = "test.csv";
        var records = new List<RawDataPoint>
        {
            new(new DateTime(2023, 10, 27, 10, 0, 0, DateTimeKind.Utc), 10.0, 100.0m),
            new(new DateTime(2023, 10, 27, 11, 0, 0, DateTimeKind.Utc), 20.0, 200.0m),
            new(new DateTime(2023, 10, 27, 12, 0, 0, DateTimeKind.Utc), 30.0, 300.0m)
        };

        var result = Result.Create(fileName, records);

        Assert.NotNull(result);
        Assert.Equal(fileName, result.FileName);
        Assert.NotEqual(Guid.Empty, result.Id);

        Assert.Equal(new DateTime(2023, 10, 27, 10, 0, 0, DateTimeKind.Utc), result.FirstOperationDate);
        Assert.Equal(TimeSpan.FromHours(2), result.TimeDelta);
        Assert.Equal(20.0, result.AverageExecutionTime);

        Assert.Equal(200.0m, result.AverageValue);
        Assert.Equal(300.0m, result.MaxValue);
        Assert.Equal(100.0m, result.MinValue);

        Assert.Equal(3, result.RawValues.Count);
        Assert.True(result.RawValues.All(rv => rv.Result == result));
    }

    [Fact]
    public void Create_WithOddNumberOfRecords_ShouldCalculateMedianCorrectly()
    {
        var baseDate = new DateTime(2023, 10, 27, 10, 0, 0, DateTimeKind.Utc);
        var records = new List<RawDataPoint>
        {
        new(baseDate, 1.0, 10m),
        new(baseDate.AddMinutes(1), 1.0, 50m),
        new(baseDate.AddMinutes(2), 1.0, 20m)
        };

        var result = Result.Create("test.csv", records);

        Assert.Equal(20m, result.MedianValue);
    }

    [Fact]
    public void Create_WithEvenNumberOfRecords_ShouldCalculateMedianCorrectly()
    {
        var records = new List<RawDataPoint>
        {
            new(DateTime.UtcNow, 1.0, 10m),
            new(DateTime.UtcNow, 1.0, 20m),
            new(DateTime.UtcNow, 1.0, 30m),
            new(DateTime.UtcNow, 1.0, 40m)
        };

        var result = Result.Create("test.csv", records);

        Assert.Equal(25m, result.MedianValue);
    }

    [Fact]
    public void Create_WithEmptyRecords_ShouldThrowArgumentException()
    {
        var emptyRecords = new List<RawDataPoint>();

        var exception = Assert.Throws<ArgumentException>(() => Result.Create("test.csv", emptyRecords));
        Assert.Equal("The record collection cannot be empty. (Parameter 'records')", exception.Message);
    }

    [Fact]
    public void Create_WithNullRecords_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            Result.Create("test.csv", (IReadOnlyList<RawDataPoint>)null!));
        Assert.Equal("The record collection cannot be empty. (Parameter 'records')", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidFileName_ShouldThrowArgumentException(string? invalidFileName)
    {
        var records = new List<RawDataPoint> { new(DateTime.UtcNow, 1, 1) };

        var exception = Assert.Throws<ArgumentException>(() => Result.Create(invalidFileName!, records));
        Assert.Equal("The file name cannot be empty. (Parameter 'fileName')", exception.Message);
    }

    [Fact]
    public void Create_WithSingleRecord_ShouldCalculateCorrectly()
    {
        var record = new RawDataPoint(DateTime.UtcNow, 15.0, 100m);
        var records = new List<RawDataPoint> { record };

        var result = Result.Create("single.csv", records);

        Assert.Equal(100m, result.MedianValue);
        Assert.Equal(100m, result.AverageValue);
        Assert.Equal(100m, result.MaxValue);
        Assert.Equal(100m, result.MinValue);
        Assert.Equal(TimeSpan.Zero, result.TimeDelta);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        var records = new List<RawDataPoint> { new(DateTime.UtcNow, 1.0, 10m) };

        var result1 = Result.Create("test1.csv", records);
        var result2 = Result.Create("test2.csv", records);

        Assert.NotEqual(result1.Id, result2.Id);
    }
}