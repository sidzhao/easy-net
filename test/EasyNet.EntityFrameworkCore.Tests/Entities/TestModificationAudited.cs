using EasyNet.Data;

namespace EasyNet.EntityFrameworkCore.Tests.Entities
{
    public class TestModificationAudited : AuditedEntity<long, long>
    {
        public string Name { get; set; }
    }
}
