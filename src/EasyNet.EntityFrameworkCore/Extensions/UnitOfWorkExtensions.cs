using System;
using EasyNet.Domain.Uow;
using EasyNet.EntityFrameworkCore.Domain.Uow;
using Microsoft.EntityFrameworkCore;

namespace EasyNet.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// Extension methods for UnitOfWork.
    /// </summary>
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        /// Gets a DbContext as a part of active unit of work.
        /// This method can be called when current unit of work is an <see cref="EfCoreUnitOfWork"/>.
        /// </summary>
        /// <param name="unitOfWork">Current (active) unit of work</param>
        public static DbContext GetDbContext(this IActiveUnitOfWork unitOfWork)
        {
            Check.NotNull(unitOfWork, nameof(unitOfWork));

            if (unitOfWork is EfCoreUnitOfWork efCoreUnitOfWork)
            {
                return efCoreUnitOfWork.GetOrCreateDbContext();
            }

            throw new ArgumentException("unitOfWork is not type of " + typeof(EfCoreUnitOfWork).FullName, nameof(unitOfWork));
        }
    }
}
