using EasyNet.Data;

namespace EasyNet.CommonTests.Common.Entities
{
    public class TestModificationAudited : AuditedEntity<long, long>
    {
        public string Name { get; set; }
    }
}
