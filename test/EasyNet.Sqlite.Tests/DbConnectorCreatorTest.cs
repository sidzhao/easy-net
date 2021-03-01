using System.Data;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Sqlite;
using EasyNet.Sqlite.Data;
using Microsoft.Extensions.Options;
using Xunit;

namespace EasyNet.SqlLite.Tests
{
    public class DbConnectorCreatorTest
    {
        [Fact]
        public void TestCreateDbConnector()
        {
            // Arrange
            var connectionString = "Filename=:memory:";
            var creator = new SqliteConnectorCreator(new OptionsWrapper<SqliteOptions>(new SqliteOptions
            {
                ConnectionString = "Filename=:memory:"
            }));

            #region Create

            // Act
            var dbConnector = creator.Create();

            // Assert
            Assert.NotNull(dbConnector.Connection);
            Assert.Null(dbConnector.Transaction);
            Assert.Equal(ConnectionState.Open, dbConnector.Connection.State);
            Assert.Same(dbConnector.Connection.ConnectionString, connectionString);

            #endregion

            #region Dispose

            // Act
            dbConnector.Dispose();

            // Assert
            Assert.Equal(ConnectionState.Closed, dbConnector.Connection.State);

            #endregion
        }

        [Fact]
        public void TestCreateDbConnectorWithTransaction()
        {
            // Arrange
            var connectionString = "Filename=:memory:";
            var creator = new SqliteConnectorCreator(new OptionsWrapper<SqliteOptions>(new SqliteOptions
            {
                ConnectionString = "Filename=:memory:"
            }));

            #region Create

            // Act
            var dbConnector = creator.Create(beginTransaction: true);

            // Assert
            Assert.NotNull(dbConnector.Connection);
            Assert.NotNull(dbConnector.Transaction);
            Assert.Same(dbConnector.Connection.ConnectionString, connectionString);
            Assert.Equal(ConnectionState.Open, dbConnector.Connection.State);
            Assert.False(dbConnector.Transaction.GetPrivateField<bool>("_completed"));

            #endregion

            #region Dispose

            // Act
            dbConnector.Dispose();

            // Assert
            Assert.True(dbConnector.Transaction.GetPrivateField<bool>("_completed"));
            Assert.Equal(ConnectionState.Closed, dbConnector.Connection.State);

            #endregion
        }
    }
}
