using System.ComponentModel.DataAnnotations.Schema;
using EasyNet.Data;

namespace EasyNet.EntityFrameworkCore.Tests.Entities
{
    [Table("Users")]
    public class User : Entity<long>, IMustHaveTenant<long>
    {
        public long TenantId { get; set; }

        public string Name { get; set; }

        public Status Status { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }

    public enum Status
    {
        Active = 0,
        Inactive = -1
    }
}
