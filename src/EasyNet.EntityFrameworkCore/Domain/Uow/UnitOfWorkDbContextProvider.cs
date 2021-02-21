using EasyNet.EntityFrameworkCore.Extensions;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;

namespace EasyNet.EntityFrameworkCore.Domain.Uow
{
    /// <summary>
    /// Implements <see cref="IDbContextProvider"/> that gets DbContext from active unit of work.
    /// </summary>
    public class UnitOfWorkDbContextProvider : IDbContextProvider
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public UnitOfWorkDbContextProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public DbContext GetDbContext()
        {
            return _currentUnitOfWorkProvider.Current.GetDbContext();
        }
    }
}
