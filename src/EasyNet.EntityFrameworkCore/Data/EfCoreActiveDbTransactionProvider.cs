using System.Data;
using EasyNet.Data;
using EasyNet.EntityFrameworkCore.Domain.Uow;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EasyNet.EntityFrameworkCore.Data
{
    public class EfCoreActiveDbTransactionProvider : IActiveDbTransactionProvider
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public EfCoreActiveDbTransactionProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public IDbConnection Connection => GetCurrentDbContext()?.Database.GetDbConnection();

        public IDbTransaction Transaction => GetCurrentDbContext()?.Database.CurrentTransaction?.GetDbTransaction();

        private DbContext GetCurrentDbContext()
        {
            if (_currentUnitOfWorkProvider?.Current is EfCoreUnitOfWork efUow)
            {
                return efUow.GetOrCreateDbContext();
            }

            throw new EasyNetException($"Current unit of work is not {typeof(EfCoreUnitOfWork).AssemblyQualifiedName}.");
        }
    }
}
