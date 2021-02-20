using System;

namespace EasyNet.Ioc
{
    public interface IScopeIocResolver : IDisposable
    {
        T GetService<T>(bool required = true);

        object GetService(Type serviceType, bool required = true);

        IScopeIocResolver CreateScope();
    }
}
