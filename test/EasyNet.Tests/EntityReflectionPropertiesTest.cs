using System;
using EasyNet.Data.Entities;
using EasyNet.Data.Entities.Auditing;
using EasyNet.Data.Entities.Helper;
using Xunit;

namespace EasyNet.Tests
{
    public class EntityReflectionPropertiesTest
    {

        [Fact]
        public void TestIsSoftDelete()
        {
            // Arrange
            var erp1 = new EntityReflectionProperties(typeof(SoftDeleteEntity));
            var erp2 = new EntityReflectionProperties(typeof(MayHaveTenantEntity));

            // Assert
            Assert.True(erp1.IsSoftDelete);
            Assert.False(erp2.IsSoftDelete);
        }

        [Fact]
        public void TestIsMustHaveTenant()
        {
            // Arrange
            var erp1 = new EntityReflectionProperties(typeof(MustHaveTenantEntity));
            var erp2 = new EntityReflectionProperties(typeof(MayHaveTenantEntity));

            // Assert
            Assert.True(erp1.IsMustHaveTenant);
            Assert.Equal(typeof(decimal), erp1.TenantIdType);
            Assert.Equal(typeof(MustHaveTenantEntity).GetProperty("TenantId"), erp1.TenantIdProperty);

            Assert.False(erp2.IsMustHaveTenant);
            Assert.Null(erp2.TenantIdProperty);
            Assert.Null(erp2.TenantIdType);
        }

        [Fact]
        public void TestIsMayHaveTenant()
        {
            // Arrange
            var erp1 = new EntityReflectionProperties(typeof(MayHaveTenantEntity));
            var erp2 = new EntityReflectionProperties(typeof(MustHaveTenantEntity));

            // Assert
            Assert.True(erp1.IsMayHaveTenant);
            Assert.Equal(typeof(Guid), erp1.TenantIdType);
            Assert.Equal(typeof(MayHaveTenantEntity).GetProperty("TenantId"), erp1.TenantIdProperty);

            Assert.False(erp2.IsMayHaveTenant);
            Assert.Null(erp2.TenantIdProperty);
            Assert.Null(erp2.TenantIdType);
        }

        [Fact]
        public void TestIsCreationAudited()
        {
            // Arrange
            var erp1 = new EntityReflectionProperties(typeof(CreationAuditedEntity));
            var erp2 = new EntityReflectionProperties(typeof(SoftDeleteEntity));

            // Assert
            Assert.True(erp1.IsCreationAudited);
            Assert.Equal(typeof(short), erp1.CreationUserIdType);
            Assert.Equal(typeof(CreationAuditedEntity).GetProperty("CreatorUserId"), erp1.CreationUserIdProperty);

            Assert.False(erp2.IsCreationAudited);
            Assert.Null(erp2.CreationUserIdProperty);
            Assert.Null(erp2.CreationUserIdType);
        }

        [Fact]
        public void TestIsModifiedAudited()
        {
            // Arrange
            var erp1 = new EntityReflectionProperties(typeof(ModificationAuditedEntity));
            var erp2 = new EntityReflectionProperties(typeof(SoftDeleteEntity));

            // Assert
            Assert.True(erp1.IsModifiedAudited);
            Assert.Equal(typeof(long), erp1.ModifiedUserIdType);
            Assert.Equal(typeof(ModificationAuditedEntity).GetProperty("LastModifierUserId"), erp1.ModifiedUserIdProperty);

            Assert.False(erp2.IsModifiedAudited);
            Assert.Null(erp2.ModifiedUserIdProperty);
            Assert.Null(erp2.ModifiedUserIdType);
        }

        [Fact]
        public void TestIsDeletionAudited()
        {
            // Arrange
            var erp1 = new EntityReflectionProperties(typeof(DeletionAuditedEntity));
            var erp2 = new EntityReflectionProperties(typeof(SoftDeleteEntity));

            // Assert
            Assert.True(erp1.IsDeletionAudited);
            Assert.Equal(typeof(int), erp1.DeleterUserIdType);
            Assert.Equal(typeof(DeletionAuditedEntity).GetProperty("DeleterUserId"), erp1.DeleterUserIdProperty);

            Assert.False(erp2.IsDeletionAudited);
            Assert.Null(erp2.DeleterUserIdProperty);
            Assert.Null(erp2.DeleterUserIdType);
        }

        private class SoftDeleteEntity : ISoftDelete
        {
            public bool IsDeleted { get; set; }
        }

        private class MustHaveTenantEntity : IMustHaveTenant<decimal>
        {
            public decimal TenantId { get; set; }
        }

        private class MayHaveTenantEntity : IMayHaveTenant<Guid>
        {
            public Guid? TenantId { get; set; }
        }

        private class CreationAuditedEntity : ICreationAudited<short>
        {
            public DateTime CreationTime { get; set; }
            public short? CreatorUserId { get; set; }
        }

        private class ModificationAuditedEntity : IModificationAudited<long>
        {
            public DateTime? LastModificationTime { get; set; }
            public long? LastModifierUserId { get; set; }
        }

        private class DeletionAuditedEntity : IDeletionAudited
        {
            public bool IsDeleted { get; set; }
            public DateTime? DeletionTime { get; set; }
            public int? DeleterUserId { get; set; }
        }
    }
}
