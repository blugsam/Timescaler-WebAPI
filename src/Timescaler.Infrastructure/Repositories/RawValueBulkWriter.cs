using Npgsql;
using NpgsqlTypes;
using Microsoft.EntityFrameworkCore;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;
using Timescaler.Infrastructure.Data;

namespace Timescaler.Infrastructure.Repositories;

public class RawValueBulkWriter : IRawValueBulkWriter
{
    private readonly TimescalerDbContext _dbContext;

    public RawValueBulkWriter(TimescalerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BulkInsertAsync(IReadOnlyCollection<RawValueDto> values, CancellationToken ct = default)
    {
        if (values.Count == 0) return;

        var connection = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
        await connection.OpenAsync(ct);

        await using var writer = await connection.BeginBinaryImportAsync(
            "COPY \"Values\" (\"Id\", \"Date\", \"ExecutionTime\", \"Value\", \"ResultEntryId\") FROM STDIN (FORMAT BINARY)", ct);

        foreach (var value in values)
        {
            await writer.StartRowAsync(ct);
            await writer.WriteAsync(value.Id, NpgsqlDbType.Uuid, ct);
            await writer.WriteAsync(value.Date, NpgsqlDbType.TimestampTz, ct);
            await writer.WriteAsync(value.ExecutionTime, NpgsqlDbType.Double, ct);
            await writer.WriteAsync(value.Value, NpgsqlDbType.Numeric, ct);
            await writer.WriteAsync(value.ResultId, NpgsqlDbType.Uuid, ct);
        }

        await writer.CompleteAsync(ct);
        await connection.CloseAsync();
    }
}
