using System;
using System.Collections.Generic;
using EasyNet.Data;
using EasyNet.EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable once CheckNamespace
namespace EasyNet.Extensions.DependencyInjection
{
    public static class EfCoreDbConnectorExtensions
    {
        public static DbContext GetDbContext(this IDbConnector dbConnector)
        {
            Check.NotNull(dbConnector, nameof(dbConnector));

            if (dbConnector is EfCoreDbConnector efCoreDbConnector)
            {
                return efCoreDbConnector.DbContext;
            }

            throw new EasyNetException($"The IDbConnector is not {typeof(EfCoreDbConnector)}.");
        }

        public static IDbContextTransaction GetDbContextTransaction(this IDbConnector dbConnector)
        {
            Check.NotNull(dbConnector, nameof(dbConnector));

            if (dbConnector is EfCoreDbConnector efCoreDbConnector)
            {
                return efCoreDbConnector.DbContextTransaction;
            }

            throw new EasyNetException($"The IDbConnector is not {typeof(EfCoreDbConnector)}.");
        }
    }
}
