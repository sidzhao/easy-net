using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EasyNet.Data;

namespace EasyNet.EntityFrameworkCore.Tests.Entities
{
    [Table("Roles")]
    public class Role : Entity, IMayHaveTenant<long>
    {
        public long? TenantId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
