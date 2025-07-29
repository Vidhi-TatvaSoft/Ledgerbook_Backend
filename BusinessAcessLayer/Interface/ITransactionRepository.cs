namespace BusinessAcessLayer.Interface;

public interface ITransactionRepository
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
