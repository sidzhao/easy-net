using System.Data;
using EasyNet.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace EasyNet.SqlLite.Data
{
    public class SqlLiteConnectorCreator : IDbConnectorCreator
    {
        protected readonly SqlLiteOptions Options;

        public SqlLiteConnectorCreator(IOptions<SqlLiteOptions> options)
        {
            Options = options.Value;
        }

        public IDbConnector Create(bool beginTransaction = false, IsolationLevel? isolationLevel = null)
        {
            var dbConnector = new DbConnector
            {
                Connection = new SqliteConnection(Options.ConnectionString)
            };

            dbConnector.Connection.Open();

            if (beginTransaction)
            {
                dbConnector.Transaction = isolationLevel != null ?
                    dbConnector.Connection.BeginTransaction(isolationLevel.Value) :
                    dbConnector.Connection.BeginTransaction();
            }

            return dbConnector;
        }
    }
}
