using EasyNet.Data;
using EasyNet.Uow;

namespace EasyNet.CommonTests.Common
{
    public class TestCurrentDbConnectorProvider : AsyncLocalCurrentDbConnectorProvider
    {
        private bool _createdTable;

        public TestCurrentDbConnectorProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IDbConnectorCreator dbConnectorCreator) : base(currentUnitOfWorkProvider, dbConnectorCreator)
        {
        }

        public override IDbConnector GetOrCreate()
        {
            var dbConnector =  base.GetOrCreate();

            if (!_createdTable)
            {
                DatabaseHelper.InitData(dbConnector.Connection);
                _createdTable = true;
            }

            return dbConnector;
        }
    }
}
