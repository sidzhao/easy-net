using System.ComponentModel.DataAnnotations.Schema;
using EasyNet.Data;

namespace EasyNet.CommonTests.Common.Entities
{
    [Table("Roles")]
    public class Role : Entity, IMayHaveTenant<long>
    {
        public long? TenantId { get; set; }

        public string Name { get; set; }
    }
}
