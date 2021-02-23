namespace EasyNet.Data.Tests.Core.Data.Entities
{
    public class TestModificationAudited : AuditedEntity<long, long>
    {
        public string Name { get; set; }
    }
}
