using EasyNet.Data;

namespace EasyNet.EntityFrameworkCore.Tests.Entities
{
    public class TestDeletionAudited : FullAuditedEntity
    {
        public bool IsActive { get; set; }
    }
}
