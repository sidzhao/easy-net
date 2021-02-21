using EasyNet.EntityFrameworkCore.Domain.Uow;
using EasyNet.Runtime.Initialization;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;

namespace EasyNet.EntityFrameworkCore.Initialization
{
    [UnitOfWork(false)]
    public class DatabaseMigrationJob : IEasyNetInitializationJob
    {
        private readonly IDbContextProvider _dbContextProvider;

        public DatabaseMigrationJob(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public void Start()
        {
            _dbContextProvider.GetDbContext().Database.Migrate();
        }
    }
}
