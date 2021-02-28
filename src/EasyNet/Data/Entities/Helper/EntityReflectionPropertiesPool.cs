using System;
using System.Collections.Concurrent;

namespace EasyNet.Data.Entities.Helper
{
    internal static class EntityReflectionPropertiesPool
    {
        private static readonly ConcurrentDictionary<string, EntityReflectionProperties> EntityReflectionProperties = new ConcurrentDictionary<string, EntityReflectionProperties>();

        public static EntityReflectionProperties GetOrAdd<TEntity>()
        {
            return GetOrAdd(typeof(TEntity));
        }

        public static EntityReflectionProperties GetOrAdd(Type entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            return EntityReflectionProperties.GetOrAdd(
                entityType.FullName ?? 
                string.Empty, s => new EntityReflectionProperties(entityType));
        }
    }
}
