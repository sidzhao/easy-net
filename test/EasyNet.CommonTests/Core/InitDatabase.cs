using System.Data;

namespace EasyNet.CommonTests.Core
{
    public static class DatabaseHelper
    {
        public static void InitData(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            // Create tables
            ExecuteNonQuery(connection,
                "CREATE TABLE Roles" +
                "(" +
                "Id INTEGER primary key autoincrement," +
                "TenantId bigint NULL," +
                "Name varchar(100) NOT NULL" +
                ")");
            ExecuteNonQuery(connection,
                "CREATE TABLE Users" +
                "(" +
                "Id INTEGER primary key autoincrement," +
                "TenantId bigint NOT NULL," +
                "Name varchar(100) NOT NULL," +
                "Status int NOT NULL," +
                "RoleId bigint NOT NULL" +
                ")");
            ExecuteNonQuery(connection,
                "CREATE TABLE TestCreationAudited" +
                "(" +
                "Id INTEGER primary key autoincrement," +
                "Name varchar(100) NOT NULL," +
                "CreationTime datetime NOT NULL," +
                "CreatorUserId bigint NULL" +
                ")");
            ExecuteNonQuery(connection,
                "CREATE TABLE TestModificationAudited" +
                "(" +
                "Id INTEGER primary key autoincrement," +
                "Name varchar(100) NOT NULL," +
                "CreationTime datetime NOT NULL," +
                "CreatorUserId bigint NULL," +
                "LastModificationTime datetime NULL," +
                "LastModifierUserId bigint NULL" +
                ")");
            ExecuteNonQuery(connection,
                "CREATE TABLE TestDeletionAudited" +
                "(" +
                "Id INTEGER primary key autoincrement," +
                "IsDeleted bit NOT NULL," +
                "IsActive bit NOT NULL," +
                "CreationTime datetime NOT NULL," +
                "CreatorUserId bigint NULL," +
                "LastModificationTime datetime NULL," +
                "LastModifierUserId bigint NULL," +
                "DeletionTime datetime NULL," +
                "DeleterUserId bigint NULL" +
                ")");

            // Insert default roles
            ExecuteNonQuery(connection, "INSERT INTO Roles(TenantId, Name) VALUES(1, 'Admin')");
            ExecuteNonQuery(connection, "INSERT INTO Roles(TenantId, Name) VALUES(1, 'Admin1')");
            ExecuteNonQuery(connection, "INSERT INTO Roles(TenantId, Name) VALUES(2, 'User')");
            ExecuteNonQuery(connection, "INSERT INTO Roles(TenantId, Name) VALUES(3, 'Client')");

            // Insert default uses
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(1, 'User1', 0, 1)");
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(1, 'User2', -1, 1)");
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(2, 'User3', 0, 2)");
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(2, 'User4', -1, 2)");
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(2, 'User5', 0, 2)");
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(3, 'User6', 0, 4)");

            // Insert default test creation audited records.
            ExecuteNonQuery(connection, "INSERT INTO TestCreationAudited(Name, CreationTime) VALUES('Create1', '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestCreationAudited(Name, CreationTime) VALUES('Create2', '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestCreationAudited(Name, CreationTime) VALUES('Create3', '2021-1-1')");

            // Insert default test modification audited records.
            ExecuteNonQuery(connection, "INSERT INTO TestModificationAudited(Name, CreationTime) VALUES('Update1', '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestModificationAudited(Name, CreationTime) VALUES('Update2', '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestModificationAudited(Name, CreationTime) VALUES('Update3', '2021-1-1')");

            // Insert default test deletion audited records.
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted, CreationTime) VALUES(1, 0, '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted, CreationTime) VALUES(0, 0, '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted, CreationTime) VALUES(1, 0, '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted, CreationTime) VALUES(1, 0, '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted, CreationTime) VALUES(0, 0, '2021-1-1')");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted, CreationTime) VALUES(0, 1, '2021-1-1')");
        }

        private static int ExecuteNonQuery(IDbConnection connection, string sql)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            return command.ExecuteNonQuery();
        }
    }
}
