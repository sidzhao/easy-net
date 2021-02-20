using Microsoft.EntityFrameworkCore;

namespace EasyNet.EntityFrameworkCore.Domain.Uow
{
    public interface IDbContextProvider
    {
        DbContext GetDbContext();
    }
}
