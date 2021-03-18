using EasyNet.CommonTests.Common;
using EasyNet.Data.Tests.Base;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace EasyNet.Data.Tests
{
    public class EfCoreTransactionTest : TransactionTest
    {
        protected override bool IsEfCore => true;

        protected override string ConnectionString =>
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EasyNetTest;Integrated Security=True;";

        protected override string PrefixName => "EFCore";

        protected override void Use(EasyNetOptions options)
        {
            options.UseEfCore<EfCoreContext>(o =>
            {
                o.UseSqlServer(ConnectionString);
            }).AsDefault<EfCoreContext>();
        }
    }
}
