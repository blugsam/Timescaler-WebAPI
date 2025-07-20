namespace Timescaler.Application.Contracts;

public record ImportSummaryDto(
    bool IsSuccess,
    string FileName,
    int RowsImported,
    Guid? ResultId,
    string? ErrorMessage = null
);