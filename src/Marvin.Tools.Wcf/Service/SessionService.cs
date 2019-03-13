using System;
using System.ServiceModel;
using Marvin.Container;
using Marvin.Logging;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Baseclass of a WebService without callback
    /// </summary>
    /// <typeparam name="TManager">Type of the service manager</typeparam>
    public abstract class SessionService<TManager> : ISessionService
        where TManager : class, IServiceManager
    {
        /// <summary>
        /// Module logger injected by Castle
        /// </summary>
        [UseChild("WcfService")]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Reference to service Manager injected by Castle
        /// </summary>
        public TManager ServiceManager { get; set; }

        /// <summary>
        /// Internal wcf channel of this connection
        /// </summary>
        private IContextChannel _channel;

        /// <see cref="ISessionService.ClientId"/>
        public string ClientId { get; private set; }

        /// <see cref="ISessionService.Subscribe(string)"/>
        public virtual void Subscribe(string clientId)
        {
            var context = OperationContext.Current;

            Logger.Log(LogLevel.Info, "New subscription for client '{0}'", clientId);

            _channel = context.Channel;
            _channel.Closed += OnChannelClosed;
            _channel.Faulted += OnChannelFaulted;

            ClientId = clientId;

            ServiceManager.Register(this);
        }

        /// <see cref="ISessionService.Heartbeat(long)"/>
        public long Heartbeat(long beat)
        {
            return beat;
        }

        /// <see cref="ISessionService.Unsubscribe()"/>
        public void Unsubscribe()
        {
            Logger.Log(LogLevel.Info, "Removing subscription for client '{0}'", ClientId);

            CleanUp(false);
        }

        /// <see cref="ISessionService.Close()"/>
        public void Close()
        {
            Logger.Log(LogLevel.Info, "Closing channel for client '{0}'", ClientId);

            CleanUp(false);
        }

        /// <summary>
        /// <c>True</c>, if the service is already closed, <c>false</c> otherwise.
        /// </summary>
        protected bool Closed;

        /// <summary>
        /// Closes this WCF Service instance.
        /// </summary>
        /// <param name="isFaulted">if set to <c>true</c> [is faulted].</param>
        protected virtual void CleanUp(bool isFaulted)
        {
            // Similar to IDisposable we will only execute unsubscribe once, whatever comes first
            lock (this)
            {
                if (Closed)
                    return;

                // Clean up internal connections
                Closed = true;
                ServiceManager.Unregister(this);

                // Close wcf channel
                _channel.Closed -= OnChannelClosed;
                _channel.Faulted -= OnChannelFaulted;

                try
                {
                    // abort in every case so the client gets a message in every case that the connection is not available anymore
                    _channel.Abort();
                }
                finally
                {
                    _channel = null;
                }
            }
        }

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            Logger.Log(LogLevel.Warning, "{0} channel to client '{1}' faulted.", GetType().Name, ClientId);

            CleanUp(true);
        }

        private void OnChannelClosed(object sender, EventArgs e)
        {
            Logger.Log(LogLevel.Info, "{0} channel to client '{1}' closed.", GetType().Name, ClientId);

            CleanUp(true);
        }
    }

    /// <summary>
    /// Baseclass of a WebService with callback
    /// </summary>
    /// <typeparam name="TCallback">Type of the WebService callback interface</typeparam>
    /// <typeparam name="TManager">Type of the service manager</typeparam>
    public abstract class SessionService<TCallback, TManager> : SessionService<TManager>
        where TCallback : class
        where TManager : class, IServiceManager
    {
        /// <summary>
        ///  Callback instance
        /// </summary>
        protected TCallback Callback;

        /// <see cref="ISessionService.Subscribe(string)"/>
        public override void Subscribe(string clientId)
        {
            var context = OperationContext.Current;

            Callback = context.GetCallbackChannel<TCallback>();

            base.Subscribe(clientId);
        }

        /// <summary>
        /// Closes this WCF Service instance.
        /// </summary>
        /// <param name="isFaulted">if set to <c>true</c> [is faulted].</param>
        protected override void CleanUp(bool isFaulted)
        {
            base.CleanUp(isFaulted);

            Callback = null;
        }
    }
}
