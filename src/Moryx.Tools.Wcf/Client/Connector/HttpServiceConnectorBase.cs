// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ServiceModel;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Base class to connect to an http web service
    /// </summary>
    /// <typeparam name="TWcfClient">The type of the WCF client.</typeparam>
    /// <typeparam name="TWcfInterface">The type of the WCF interface.</typeparam>
    public abstract class HttpServiceConnectorBase<TWcfClient, TWcfInterface> : IHttpServiceConnector, IDisposable
        where TWcfInterface : class
        where TWcfClient : ClientBase<TWcfInterface>, TWcfInterface
    {
        #region Dependency

        /// <summary>
        /// Factory to connect to the given wcf client.
        /// </summary>
        public IWcfClientFactory ClientFactory { get; set; }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Identifier of the client instance
        /// </summary>
        protected long ClientId { get; set; }

        private bool _isAvailable;

        /// <summary>
        /// The client to communicate over the wcf service
        /// </summary>
        protected TWcfClient WcfClient { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current client version of the wcf client
        /// </summary>
        protected abstract string ClientVersion { get; }

        #endregion

        ///
        public virtual void Start()
        {
            IsAvailable = false;

            //Instanciate client and get client info
            ClientId = ClientFactory.Create<TWcfClient, TWcfInterface>(ClientVersion, ClientCallback);
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
            //TODO: Distinguish between IDisposable.Dispose() and Stop()
        }

        ///
        public virtual void Dispose()
        {
            if (WcfClient == null)
                return;

            //Close connection
            IsAvailable = false;

            if (ClientId != 0)
                ClientFactory.Destroy(ClientId);
        }

        ///
        protected virtual void ClientCallback(ConnectionState state, TWcfClient client)
        {
            switch (state)
            {
                case ConnectionState.New:
                    return;
                case ConnectionState.Success:
                    WcfClient = client;
                    IsAvailable = true;
                    break;
                case ConnectionState.FailedTry:
                case ConnectionState.VersionMissmatch:
                case ConnectionState.ConnectionLost:
                    IsAvailable = false;
                    break;
                case ConnectionState.Closing:
                case ConnectionState.Closed:
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state));
            }
        }

        ///
        public event EventHandler AvailabilityChanged;

        /// <summary>
        /// Gets a value indicating whether this controller is available.
        /// </summary>
        public bool IsAvailable
        {
            get { return _isAvailable; }
            private set
            {
                _isAvailable = value;

                var handler = AvailabilityChanged;
                handler?.Invoke(this, new EventArgs());
            }
        }
    }
}
