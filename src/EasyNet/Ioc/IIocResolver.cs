using System;

namespace EasyNet.Ioc
{
    public interface IIocResolver
    {
        T GetService<T>(bool required = true);

        object GetService(Type serviceType, bool required = true);

        IScopeIocResolver CreateScope();
    }
}
