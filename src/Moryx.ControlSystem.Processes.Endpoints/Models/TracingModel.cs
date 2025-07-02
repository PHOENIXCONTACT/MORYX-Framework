using System;
using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.ControlSystem.Processes.Endpoints
{
    [DataContract]
    public class TracingModel
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public DateTime? Started { get; set; }

        [DataMember]
        public DateTime? Completed { get; set; }

        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public Entry Properties { get; set; }
    }
}