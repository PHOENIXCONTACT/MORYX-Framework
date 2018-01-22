using System.ComponentModel;
using Marvin.AbstractionLayer.Resources;
using Marvin.Resources.Management;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    public class RoutingResource : PublicResource
    {
        [EditorVisible, ResourceTypes(typeof(IWpc))]
        [Description("Type of wpc for Autocreate")]
        public string WpcType { get; set; }

        public void AutoCreateWpc()
        {
            var wpc = Creator.Instantiate<IWpc>(WpcType);
        }
    }


    public interface IWpc : IResource
    {
    }

    public class Wpc : Resource, IWpc
    {
    }
}