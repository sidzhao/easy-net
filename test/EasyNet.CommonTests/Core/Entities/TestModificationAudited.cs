using EasyNet.Data;

namespace EasyNet.CommonTests.Core.Entities
{
    public class TestModificationAudited : AuditedEntity<long, long>
    {
        public string Name { get; set; }
    }
}
