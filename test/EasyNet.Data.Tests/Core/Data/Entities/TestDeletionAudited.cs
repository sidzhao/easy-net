namespace EasyNet.Data.Tests.Core.Data.Entities
{
    public class TestDeletionAudited : FullAuditedEntity<int, long>
    {
        public bool IsActive { get; set; }
    }
}
