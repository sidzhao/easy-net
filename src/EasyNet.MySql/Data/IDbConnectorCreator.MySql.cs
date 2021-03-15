using System.Data;
using EasyNet.Data;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace EasyNet.MySql.Data
{
    public class MySqlConnectorCreator : IDbConnectorCreator
    {
        protected readonly MySqlOptions Options;

        public MySqlConnectorCreator(IOptions<MySqlOptions> options)
        {
            Options = options.Value;
        }

        public IDbConnector Create(bool beginTransaction = false, IsolationLevel? isolationLevel = null)
        {
            var dbConnector = new DbConnector
            {
                Connection = new MySqlConnection(Options.ConnectionString)
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
