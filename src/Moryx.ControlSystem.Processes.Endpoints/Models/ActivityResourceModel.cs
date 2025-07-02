using System.Runtime.Serialization;

namespace Moryx.ControlSystem.Processes.Endpoints
{
    [DataContract]
    public class ActivityResourceModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
