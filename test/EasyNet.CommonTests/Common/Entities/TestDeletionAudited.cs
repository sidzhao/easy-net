using EasyNet.Data;

namespace EasyNet.CommonTests.Common.Entities
{
    public class TestDeletionAudited : FullAuditedEntity<int, long>
    {
        public bool IsActive { get; set; }
    }
}
