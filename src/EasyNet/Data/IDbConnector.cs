using System;
using System.Data;

namespace EasyNet.Data
{
    public interface IDbConnector : IDisposable
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }
    }
}
