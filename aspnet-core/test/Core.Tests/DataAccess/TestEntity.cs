using AcWebTool.Core.DataAccess;

namespace AcWebTool.Core.Tests.DataAccess
{
    public class TestEntity : IEntity
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 1123
                ^ Message?.GetHashCode() ?? 6434
                ^ 52234;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is TestEntity te)) return false;
            return Id == te.Id && Message == te.Message;
        }
    }
}
