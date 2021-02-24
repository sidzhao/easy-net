using System;

namespace EasyNet
{
    public class EasyNetRepositoryBuilder
    {
        public EasyNetRepositoryBuilder(EasyNetOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public EasyNetOptions Options { get; }
    }
}