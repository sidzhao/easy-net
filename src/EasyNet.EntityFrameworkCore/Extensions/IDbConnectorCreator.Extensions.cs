using System.Data;
using EasyNet.Data;
using EasyNet.EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace EasyNet.Extensions.DependencyInjection
{
    public static class EfCoreDbConnectorCreatorExtensions
    {
        public static IDbConnector Create(this IDbConnectorCreator dbConnectorCreator, IsolationLevel isolationLevel)
        {
            Check.NotNull(dbConnectorCreator, nameof(dbConnectorCreator));

            var dbConnector = dbConnectorCreator.Create();
            if (dbConnector is EfCoreDbConnector efCoreDbConnector)
            {
                efCoreDbConnector.DbContextTransaction = efCoreDbConnector.DbContext.Database.BeginTransaction(isolationLevel);

                return efCoreDbConnector;
            }

            throw new EasyNetException($"The interface {typeof(IDbConnector).AssemblyQualifiedName} is not implemented with class {typeof(EfCoreDbConnector)}.");
        }
    }
}
