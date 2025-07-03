using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints
{
    [DataContract]
    public class ReportModel
    {
        [DataMember]
        public ConfirmationType ConfirmationType { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public int SuccessCount { get; set; }

        [DataMember]
        public int FailureCount { get; set; }

        [DataMember]
        public string UserIdentifier { get; set; }
    }
}