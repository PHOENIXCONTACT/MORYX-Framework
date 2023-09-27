using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Resources.Management.Tests
{
    public interface IGenericMethodCall<T> : IResource
    {
        /// <summary>
        /// Get channel using specialized API
        /// </summary>
        IList<TChannel> GenericMethod<TChannel>(string identifier);
    }

    public class ResourceWithGenericMethod : Driver, IGenericMethodCall<object>
    {
        public IList<TChannel> GenericMethod<TChannel>(string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
