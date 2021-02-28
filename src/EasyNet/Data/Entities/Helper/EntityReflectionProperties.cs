using System;
using System.Reflection;
using EasyNet.Data.Entities.Auditing;
using EasyNet.Extensions.DependencyInjection;

namespace EasyNet.Data.Entities.Helper
{
    internal class EntityReflectionProperties
    {
        public EntityReflectionProperties(Type entityType)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; set; }

        public bool IsSoftDelete
        {
            get
            {
                if (_isSoftDelete == null)
                {
                    _isSoftDelete = typeof(ISoftDelete).IsAssignableFrom(EntityType);
                }

                return _isSoftDelete.Value;
            }
        }
        private bool? _isSoftDelete;

        #region Tenant

        public bool IsMustHaveTenant
        {
            get
            {
                if (_isMustHaveTenant != null) return _isMustHaveTenant.Value;

                var tenantGeneric = EntityType.GetImplementedRawGeneric(typeof(IMustHaveTenant<>));
                if (tenantGeneric == null)
                {
                    _isMustHaveTenant = false;
                    TenantIdType = null;
                }
                else
                {
                    _isMustHaveTenant = true;
                    TenantIdType = tenantGeneric.GenericTypeArguments[0];
                    TenantIdProperty = EntityType.GetProperty("TenantId");

                    if (TenantIdProperty == null) throw new EasyNetException($"Cannot found property TenantId in entity {EntityType.AssemblyQualifiedName}.");
                }

                return _isMustHaveTenant.Value;
            }
        }
        private bool? _isMustHaveTenant;

        public bool IsMayHaveTenant
        {
            get
            {
                if (_isMayHaveTenant != null) return _isMayHaveTenant.Value;
                var tenantGeneric = EntityType.GetImplementedRawGeneric(typeof(IMayHaveTenant<>));
                if (tenantGeneric == null)
                {
                    _isMayHaveTenant = false;
                    TenantIdType = null;
                }
                else
                {
                    _isMayHaveTenant = true;
                    TenantIdType = tenantGeneric.GenericTypeArguments[0];
                    TenantIdProperty = EntityType.GetProperty("TenantId");

                    if (TenantIdProperty == null) throw new EasyNetException($"Cannot found property TenantId in entity {EntityType.AssemblyQualifiedName}.");
                }

                return _isMayHaveTenant.Value;
            }
        }
        private bool? _isMayHaveTenant;

        public Type TenantIdType { get; private set; }

        public PropertyInfo TenantIdProperty { get; private set; }

        #endregion

        #region CreationAudited

        public bool IsCreationAudited
        {
            get
            {
                if (_isCreationAudited != null) return _isCreationAudited.Value;
                var creationGeneric = EntityType.GetImplementedRawGeneric(typeof(ICreationAudited<>));
                if (creationGeneric == null)
                {
                    _isCreationAudited = false;
                    CreationUserIdType = null;
                    CreationUserIdProperty = null;
                }
                else
                {
                    _isCreationAudited = true;
                    CreationUserIdType = creationGeneric.GenericTypeArguments[0];
                    CreationUserIdProperty = EntityType.GetProperty("CreatorUserId");

                    if (CreationUserIdProperty == null) throw new EasyNetException($"Cannot found property CreatorUserId in entity {EntityType.AssemblyQualifiedName}.");
                }

                return _isCreationAudited.Value;
            }
        }
        private bool? _isCreationAudited;

        public Type CreationUserIdType { get; private set; }

        public PropertyInfo CreationUserIdProperty { get; private set; }

        #endregion

        #region ModifiedAudited

        public bool IsModifiedAudited
        {
            get
            {
                if (_isModifiedAudited != null) return _isModifiedAudited.Value;
                var modifiedGeneric = EntityType.GetImplementedRawGeneric(typeof(IModificationAudited<>));
                if (modifiedGeneric == null)
                {
                    _isModifiedAudited = false;
                    ModifiedUserIdType = null;
                    ModifiedUserIdProperty = null;
                }
                else
                {
                    _isModifiedAudited = true;
                    ModifiedUserIdType = modifiedGeneric.GenericTypeArguments[0];
                    ModifiedUserIdProperty = EntityType.GetProperty("LastModifierUserId");

                    if (ModifiedUserIdProperty == null) throw new EasyNetException($"Cannot found property LastModifierUserId in entity {EntityType.AssemblyQualifiedName}.");
                }

                return _isModifiedAudited.Value;
            }
        }
        private bool? _isModifiedAudited;

        public Type ModifiedUserIdType { get; private set; }

        public PropertyInfo ModifiedUserIdProperty { get; private set; }

        #endregion

        #region ModifiedAudited

        public bool IsDeletionAudited
        {
            get
            {
                if (_isDeletionAudited != null) return _isDeletionAudited.Value;
                var deletionGeneric = EntityType.GetImplementedRawGeneric(typeof(IDeletionAudited<>));
                if (deletionGeneric == null)
                {
                    _isDeletionAudited = false;
                    DeleterUserIdType = null;
                    DeleterUserIdProperty = null;
                }
                else
                {
                    _isDeletionAudited = true;
                    DeleterUserIdType = deletionGeneric.GenericTypeArguments[0];
                    DeleterUserIdProperty = EntityType.GetProperty("DeleterUserId");

                    if (DeleterUserIdProperty == null) throw new EasyNetException($"Cannot found property DeleterUserId in entity {EntityType.AssemblyQualifiedName}.");
                }

                return _isDeletionAudited.Value;
            }
        }
        private bool? _isDeletionAudited;

        public Type DeleterUserIdType { get; private set; }

        public PropertyInfo DeleterUserIdProperty { get; private set; }

        #endregion
    }
}