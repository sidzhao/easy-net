using EasyNet.Data;

namespace EasyNet.CommonTests.Entities
{
    public class TestDeletionAudited : FullAuditedEntity<int, long>
    {
        public bool IsActive { get; set; }
    }
}
