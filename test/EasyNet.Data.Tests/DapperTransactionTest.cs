using EasyNet.CommonTests.Common.Entities;
using EasyNet.Data.Tests.Base;
using EasyNet.Extensions.DependencyInjection;

namespace EasyNet.Data.Tests
{
    public class DapperTransactionTest : TransactionTest
    {
        protected override bool IsEfCore => false;

        protected override string ConnectionString =>
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EasyNetTest;Integrated Security=True;";

        protected override string PrefixName => "Dapper";

        protected override void Use(EasyNetOptions options)
        {
            options.UseSqlServer(ConnectionString);
            options.UseDapper().AsDefault(typeof(User).Assembly);
        }
    }
}
