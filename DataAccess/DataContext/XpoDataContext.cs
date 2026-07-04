using DevExpress.Xpo;

namespace Sln.DataAccess.DataContext;

public class XpoDataContext
{
    private readonly UnitOfWork _uow;

    public XpoDataContext(UnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<TResult> DoAsync<TResult>(Func<UnitOfWork, Task<TResult>> action)
    {
        return await action(_uow);
    }

    public async Task<TResult> DoTranAsync<TResult>(Func<UnitOfWork, Task<TResult>> action)
    {
        var result = await action(_uow);
        await _uow.CommitChangesAsync();
        return result;
    }
}