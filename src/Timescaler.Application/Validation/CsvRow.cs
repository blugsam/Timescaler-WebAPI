namespace Timescaler.Application.Validation;

public record CsvRow(int LineNumber, string[] Fields);