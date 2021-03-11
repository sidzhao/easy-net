using System.Data;
using System.Data.SqlClient;
using EasyNet.Data;
using Microsoft.Extensions.Options;

namespace EasyNet.SqlServer.Data
{
    public class SqlServerConnectorCreator : IDbConnectorCreator
    {
        protected readonly SqlServerOptions Options;

        public SqlServerConnectorCreator(IOptions<SqlServerOptions> options)
        {
            Options = options.Value;
        }

        public IDbConnector Create(bool beginTransaction = false, IsolationLevel? isolationLevel = null)
        {
            var dbConnector = new DbConnector
            {
                Connection = new SqlConnection(Options.ConnectionString)
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
