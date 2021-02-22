using EasyNet.Data;
using EasyNet.Uow;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace EasyNet.SqlLite.Data
{
    public class EasyNetSqlLiteConnectorCreator : IDbConnectorCreator 
    {
        protected readonly EasyNetSqlLiteOptions Options;

        public EasyNetSqlLiteConnectorCreator(IOptions<EasyNetSqlLiteOptions> options)
        {
            Options = options.Value;
        }

        public virtual IDbConnector Create(UnitOfWorkOptions uowOptions = null)
        {
            var dbConnector = new DbConnector
            {
                Connection = new SqliteConnection(Options.ConnectionString)
            };

            if (uowOptions?.IsTransactional != null && uowOptions.IsTransactional.Value)
            {
                dbConnector.Transaction = dbConnector.Connection.BeginTransaction(uowOptions.GetSystemDataIsolationLevel());
            }

            return dbConnector;
        }
    }
}
