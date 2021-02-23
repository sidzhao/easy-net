using EasyNet.Data;

namespace EasyNet.CommonTests.Core.Entities
{
    public class TestDeletionAudited : FullAuditedEntity<int, long>
    {
        public bool IsActive { get; set; }
    }
}
