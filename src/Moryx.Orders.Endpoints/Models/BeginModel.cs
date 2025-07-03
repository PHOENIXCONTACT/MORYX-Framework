using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints
{
    [DataContract]
    public class BeginModel
    {
        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public string? UserId { get; set; }
    }
}
