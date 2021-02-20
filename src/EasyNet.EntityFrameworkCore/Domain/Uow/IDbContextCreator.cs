using Microsoft.EntityFrameworkCore;

namespace EasyNet.EntityFrameworkCore.Domain.Uow
{
    public interface IDbContextCreator
    {
        DbContext CreateDbContext();
    }
}
