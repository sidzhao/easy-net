using System.ComponentModel.DataAnnotations.Schema;

namespace EasyNet.Data.Tests.Core.Data.Entities
{
    [Table("Roles")]
    public class Role : Entity, IMayHaveTenant<long>
    {
        public long? TenantId { get; set; }

        public string Name { get; set; }
    }
}
