using EasyNet.Data.Entities.Auditing;

namespace EasyNet.CommonTests.Common.Entities
{
    public class TestCreationAudited : CreationAuditedEntity
    {
        public string Name { get; set; }
    }
}
