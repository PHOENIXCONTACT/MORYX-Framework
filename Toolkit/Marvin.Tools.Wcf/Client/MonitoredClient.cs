using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Marvin.Tools.Wcf
{
    internal delegate ClientCredentials GetClientCredentials(ICommunicationObject client);
    internal delegate IClientChannel GetInnerChannel(ICommunicationObject client);

    internal class MonitoredClient
    {
        public IClientConfig Config { get; set; }

        public ServiceConfiguration ServiceConfiguration { get; set; }

        public Binding Binding { get; set; }

        public Type ClientType { get; set; }

        public string ServiceName { get; set; }

        public Action<ConnectionState, ICommunicationObject> ConnectionCallback { get; set; }

        public GetClientCredentials GetClientCredentials { get; set; }

        public GetInnerChannel GetInnerChannel { get; set; }

        public object CallbackService { get; set; }

        public InstanceContext CallbackContext { get; set; }

        public IClientChannel InnerChannel { get; set; }

        public ICommunicationObject Instance { get; set; }

        public InternalConnectionState State { get; set; }

        public WcfClientInfo ClientInfo { get; set; }

        public bool Destroy { get; set; }
    }
}