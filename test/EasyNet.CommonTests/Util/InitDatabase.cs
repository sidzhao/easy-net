using System.Data;

namespace EasyNet.CommonTests.Util
{
    public static class DatabaseHelper
    {
        public static void InitData(IDbConnection connection)
        {
            connection.Open();

            // Create tables
            ExecuteNonQuery(connection,
                "CREATE TABLE Roles" +
                "(" +
                "Id bigint IDENTITY primary key," +
                "TenantId bigint NULL," +
                "Name varchar(100) NOT NULL" +
                ")");
            ExecuteNonQuery(connection,
                "CREATE TABLE Users" +
                "(" +
                "Id bigint IDENTITY primary key," +
                "TenantId bigint NOT NULL," +
                "Name varchar(100) NOT NULL," +
                "Status int NOT NULL," +
                "RoleId bigint NOT NULL" +
                ")");
            ExecuteNonQuery(connection,
                "CREATE TABLE TestModificationAudited" +
                "(" +
                "Id bigint IDENTITY primary key," +
                "Name varchar(100) NOT NULL," +
                "CreationTime datetime NULL," +
                "CreatorUserId bigint NULL," +
                "LastModificationTime datetime NULL," +
                "LastModifierUserId bigint NULL" +
                ")");
            ExecuteNonQuery(connection,
                "CREATE TABLE TestDeletionAudited" +
                "(" +
                "Id int IDENTITY primary key," +
                "IsDeleted bit NOT NULL," +
                "IsActive bit NOT NULL," +
                "CreationTime datetime NULL," +
                "CreatorUserId bigint NULL," +
                "LastModificationTime datetime NULL," +
                "LastModifierUserId bigint NULL," +
                "DeletionTime datetime NULL," +
                "DeleterUserId bigint NULL" +
                ")");

            // Insert default roles
            ExecuteNonQuery(connection, "INSERT INTO Roles(TenantId, Name) VALUES(1, 'Admin')");
            ExecuteNonQuery(connection, "INSERT INTO Roles(TenantId, Name) VALUES(1, 'User')");

            // Insert default uses
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(1, 'User1', 0, 1)");
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(1, 'User2', 0, 2)");
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(1, 'User3', -1, 2)");
            ExecuteNonQuery(connection, "INSERT INTO Users(TenantId, Name, Status, RoleId) VALUES(2, 'User4', 0, 2)");

            // Insert default test modification audited records.
            ExecuteNonQuery(connection, "INSERT INTO TestModificationAudited(Name) VALUES('Update1')");
            ExecuteNonQuery(connection, "INSERT INTO TestModificationAudited(Name) VALUES('Update2')");
            ExecuteNonQuery(connection, "INSERT INTO TestModificationAudited(Name) VALUES('Update3')");

            // Insert default test deletion audited records.
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted) VALUES(1, 0)");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted) VALUES(0, 0)");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted) VALUES(1, 0)");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted) VALUES(1, 0)");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted) VALUES(0, 0)");
            ExecuteNonQuery(connection, "INSERT INTO TestDeletionAudited(IsActive, IsDeleted) VALUES(0, 1)");


            connection.Close();
            connection.Dispose();
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
