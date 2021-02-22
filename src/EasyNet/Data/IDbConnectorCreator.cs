using EasyNet.Uow;

namespace EasyNet.Data
{
    public interface IDbConnectorCreator
    {
        IDbConnector Create(UnitOfWorkOptions uowOptions = null);
    }
}
