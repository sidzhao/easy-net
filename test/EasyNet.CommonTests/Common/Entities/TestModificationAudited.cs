using EasyNet.Data.Entities.Auditing;

namespace EasyNet.CommonTests.Common.Entities
{
    public class TestModificationAudited : AuditedEntity<long, long>
    {
        public string Name { get; set; }
    }
}
