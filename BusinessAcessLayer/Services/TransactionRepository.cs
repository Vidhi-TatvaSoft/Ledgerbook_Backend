using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace BusinessAcessLayer.Services;

public class TransactionRepository : ITransactionRepository
{
    private readonly LedgerBookDbContext _context;
    private IDbContextTransaction? _transaction;

    public TransactionRepository(LedgerBookDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
            await DisposeAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await DisposeAsync();
        }
    }

    private async Task DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
