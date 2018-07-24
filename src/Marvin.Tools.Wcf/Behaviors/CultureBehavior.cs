using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Behavior for the endpoint which adds a <see cref="CultureMessageInspector"/>
    /// which sets and sends the correct UI culture
    /// </summary>
    public class CultureBehavior : IEndpointBehavior
    {
        //Source: https://www.codeproject.com/Articles/21504/Automatic-Culture-Flowing-with-WCF-by-using-Custom

        /// <inheritdoc />
        public void AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <inheritdoc />
        public void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new CultureMessageInspector());
        }

        /// <inheritdoc />
        public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CultureMessageInspector());
        }

        /// <inheritdoc />
        public void Validate(System.ServiceModel.Description.ServiceEndpoint endpoint)
        {
        }
    }

    internal class CultureMessageInspector : IDispatchMessageInspector, IClientMessageInspector
    {
        private const string HeaderKey = "CurrentUICulture";
        private const string HeaderNamespace = "Marvin.Tools.Wcf";

        /// <inheritdoc />
        object IDispatchMessageInspector.AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var headerIndex = request.Headers.FindHeader(HeaderKey, HeaderNamespace);
            if (headerIndex != -1)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(request.Headers.GetHeader<string>(headerIndex));
            }
            return null;
        }

        /// <inheritdoc />
        void IDispatchMessageInspector.BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        /// <inheritdoc />
        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            request.Headers.Add(MessageHeader.CreateHeader(HeaderKey, HeaderNamespace, Thread.CurrentThread.CurrentUICulture.Name));
            return null;
        }

        /// <inheritdoc />
        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
    }
}