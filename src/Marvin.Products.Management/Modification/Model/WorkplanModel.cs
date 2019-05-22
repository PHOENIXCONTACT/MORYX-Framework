using System.Runtime.Serialization;
using Marvin.Products.Model;
using Marvin.Workflows;

namespace Marvin.Products.Management.Modification
{
    [DataContract(IsReference = true)]
    internal class WorkplanModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public WorkplanState State { get; set; }

        internal static WorkplanModel FromWorkplan(IWorkplan wp)
        {
            return new WorkplanModel
            {
                Id = wp.Id,
                Name = wp.Name,
                Version = wp.Version,
                State = wp.State
            };   
        }
    }
}