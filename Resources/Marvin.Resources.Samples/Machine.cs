using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Resources.Management;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    [ResourceRegistration(nameof(Machine), typeof(IRootResource))]
    public class Machine : RootResource
    {
        [DataMember, EditorVisible]
        public string CurrentState { get; set; }
    }
}