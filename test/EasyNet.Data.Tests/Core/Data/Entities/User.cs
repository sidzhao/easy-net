using System.ComponentModel.DataAnnotations.Schema;

namespace EasyNet.Data.Tests.Core.Data.Entities
{
    [Table("Users")]
    public class User : Entity<long>, IMustHaveTenant<long>
    {
        public long TenantId { get; set; }

        public string Name { get; set; }

        public Status Status { get; set; }

        public long RoleId { get; set; }
    }

    public enum Status
    {
        Active = 0,
        Inactive = -1
    }
}
