using System.Data;
using FGS.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FGS.DAL.ViewModel;

public abstract class ViewModelRepositoryBase
{
    protected readonly FgsViewModelContext Context;
    protected readonly ILogger Logger;

    protected ViewModelRepositoryBase(FgsViewModelContext db, ILogger logger)
    {       
        Logger = logger;
        Context = db;
    }

    public async Task TransactionWrapperAsync(Func<Task> action, CancellationToken ct)
    {
        // todo возможно хватит и repeatable read
        await using var transaction = await Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            await action();
            await transaction.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}