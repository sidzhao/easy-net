using System.Data;

namespace EasyNet.Data
{
    public interface IDbConnectorCreator
    {
        IDbConnector Create(bool beginTransaction = false, IsolationLevel? isolationLevel = null);
    }
}
