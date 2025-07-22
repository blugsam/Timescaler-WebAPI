using Microsoft.EntityFrameworkCore.Storage;
using Timescaler.Application.Ports;
using Timescaler.Infrastructure.Data;

namespace Timescaler.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TimescalerDbContext _dbContext;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(TimescalerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(ct);
            }
        }
        catch
        {
            await RollbackAsync(ct);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _dbContext.SaveChangesAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
    }
}
