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

        [EditorVisible]
        public void AutoCreateWpc()
        {
            var wpc = (Resource)Creator.Instantiate<IWpc>(WpcType);
            wpc.Parent = this;

            var pos = Creator.Instantiate<WpcPosition>();
            pos.Parent = wpc;
            wpc.Children.Add(pos);

            Children.Add(wpc);

            RaiseResourceChanged();
        }
    }


    public interface IWpc : IResource
    {
    }

    public class Wpc : Resource, IWpc
    {
    }

    public class WpcPosition : Resource
    {

    }
}