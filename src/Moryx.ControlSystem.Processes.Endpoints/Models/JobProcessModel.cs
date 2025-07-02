using System;
using System.Runtime.Serialization;

namespace Moryx.ControlSystem.Processes.Endpoints
{
    [DataContract]
    public class JobProcessModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long RecipeId { get; set; }

        [DataMember]
        public ProcessProgress State { get; set; }

        [DataMember]
        public bool Rework { get; set; }

        [DataMember]
        public DateTime Started { get; set; }

        [DataMember]
        public DateTime Completed { get; set; }

        [DataMember]
        public ProcessActivityModel[] Activities { get; set; }

        [DataMember]
        public bool IsRunning { get; set; }
    }
}
