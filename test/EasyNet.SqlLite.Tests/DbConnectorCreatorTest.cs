using System;
using System.Data;
using EasyNet.SqlLite.Data;
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
            var creator = new EasyNetSqlLiteConnectorCreator(new OptionsWrapper<EasyNetSqlLiteOptions>(new EasyNetSqlLiteOptions
            {
                ConnectionString = "Filename=:memory:"
            }));

            #region Create

            // Act
            var dbConnector = creator.Create();
            dbConnector.Connection.Open();

            // Assert
            Assert.NotNull(dbConnector.Connection);
            Assert.Same(dbConnector.Connection.ConnectionString, connectionString);

            #endregion

            #region Dispose

            // Act
            dbConnector.Dispose();

            // Assert
            Assert.Equal(ConnectionState.Closed, dbConnector.Connection.State);

            #endregion
        }
    }
}
