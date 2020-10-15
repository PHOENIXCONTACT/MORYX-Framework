using Moryx.Serialization;

namespace Moryx.Resources.Interaction
{
    public class CreateResource
    {
        public string ResourceType { get; set; }
        
        public MethodEntry Constructor { get; set; }
    }
}
