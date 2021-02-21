using System;

namespace EasyNet.Data
{
    /// <summary>
    /// Implement this interface for an entity which must have TenantId.
    /// </summary>
    public interface IMustHaveTenant : IMustHaveTenant<int>
    {
    }

    /// <summary>
    /// Implement this interface for an entity which must have TenantId.
    /// </summary>
    public interface IMustHaveTenant<TTenantKey> where TTenantKey : IEquatable<TTenantKey>
    {
        /// <summary>
        /// TenantId of this entity.
        /// </summary>
        TTenantKey TenantId { get; set; }
    }
}
