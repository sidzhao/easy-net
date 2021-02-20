using DotNetCore.CAP.Internal;

namespace EasyNet.EventBus.Cap
{
    public class EasyNetCapSubscribeAttribute : TopicAttribute
    {
        public EasyNetCapSubscribeAttribute(string name, bool isPartial = false) : base(name, isPartial)
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
