using System.Data;
using EasyNet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EasyNet.EntityFrameworkCore.Data
{
    public class EfCoreDbConnector : IDbConnector
    {
        public DbContext DbContext { get; set; }

        public IDbContextTransaction DbContextTransaction { get; set; }

        public IDbConnection Connection => DbContext?.Database?.GetDbConnection();

        public IDbTransaction Transaction => DbContextTransaction?.GetDbTransaction();

        public void Dispose()
        {
            DbContextTransaction?.Dispose();
            DbContext?.Dispose();
        }
    }
}