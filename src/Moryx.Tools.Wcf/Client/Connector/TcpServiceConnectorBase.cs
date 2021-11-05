// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Timers;
using Moryx.Logging;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Base Wcf Connector class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public abstract class TcpServiceConnectorBase<T, K> : ITcpServiceReference
        where T : ClientBase<K>, ITcpServiceReference, K
        where K : class
    {
        #region Dependencies

        /// Logger to log several info and debug messages
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Factory to create new clients.
        /// </summary>
        public abstract IWcfClientFactory WcfClientFactory { get; set; }

        #endregion

        #region Fields, Properties and Constants

        private const int HeartbeatTimerDelay = 120000; // 120 seconds

        private bool _trying;

        /// <summary>
        /// WCF Client Instance ...
        /// </summary>
        protected T WcfClient { get; set; }

        /// <summary>
        /// The unique name of the client.
        /// </summary>
        protected string ClientId => WcfClientFactory != null ? WcfClientFactory.ClientId : string.Empty;

        #endregion

        /// <summary>
        /// Try to connect to WcfClient.
        /// </summary>
        public void TryConnect()
        {
            lock (this)
            {
                if (_trying)
                    return;

                if ((WcfClient == null) && (!IsOpened))
                {
                    CreateWcfClient();
                }

                _trying = true;
            }
        }

        #region Events

        /// <summary>
        /// Publish connected events to all the listeners.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Publish subscribed events to all the listeners.
        /// </summary>
        public event EventHandler Subscribed;

        /// <summary>
        /// Publish disconnect events to all the listeners.
        /// </summary>
        public event EventHandler Diconnected; // TODO: Rename to Disconnected in the next major

        #endregion

        /// <summary>
        /// Gets the client configuration. To be implemented in derived
        /// classes.
        /// </summary>
        /// <returns></returns>
        protected abstract ClientConfig ClientConfig { get; }

        /// <summary>
        /// Called when [subscribed].
        /// Called MANY TIMES after calling ReSubscribe()
        /// </summary>
        protected virtual void OnSubscribed()
        {
            // override in derived classes to perform
            // additional client initializations.
        }

        /// <summary>
        /// Creates the WCF client.
        /// </summary>
        private void CreateWcfClient()
        {
            WcfClientFactory.Create<T, K>(ClientConfig, this, ConnectionCallback);
        }

        private void ConnectionCallback(ConnectionState result, K client)
        {
            // Try catch is redundant since this is caught in client factory anyway
            lock (this)
            {
                switch (result)
                {
                    case ConnectionState.Success:
                        WcfClient = (T) client;
                        Logger.Log(LogLevel.Info, "WCF client for '{0}' successfully created, client state '{1}'.",
                            ClientConfig.Endpoint, WcfClient.State);
                        StartHeartbeat();
                        RaiseConnected();
                        // Subscribe client and
                        WcfClient.Subscribe(ClientId);
                        RaiseOnReSubscribe();
                        break;

                    case ConnectionState.Closing:
                        // Application is closing and giving us a last chance to inform the server
                        StopHeartbeat();
                        WcfClient.UnSubscribe();
                        WcfClient = null;
                        break;

                    case ConnectionState.ConnectionLost:
                        Logger.Log(LogLevel.Warning, "Connection to '{0}' lost.", ClientConfig.Endpoint, result);
                        StopHeartbeat();
                        RaiseDisconnected();
                        WcfClient = null;
                        break;

                    case ConnectionState.Closed:
                        Logger.Log(LogLevel.Warning, "Connection to '{0}' closed.", ClientConfig.Endpoint, result);
                        StopHeartbeat();
                        RaiseDisconnected();
                        WcfClient = null;
                        break;

                    default:
                        Logger.Log(LogLevel.Warning, "WCF client for '{0}' could not be created, result '{1}'.",
                            ClientConfig.Endpoint, result);
                        break;
                }
            }
        }

        /// <summary>
        /// Subscribes the specified client identifier to the server.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        public void Subscribe(string clientId)
        {
            // use ReSubscribe()!
            return;
        }

        /// <summary>
        /// Subscribes the specified client identifier to the server asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>A Task which represents the asynchronous operation.</returns>
        public Task SubscribeAsync(string clientId)
        {
            // use ReSubscribe()!
            return null;
        }

        /// <summary>
        /// Try to resubscribe to the Wcf Client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        public void ReSubscribe(string clientId)
        {
            Logger.Log(LogLevel.Debug, "ReSubscribe '{0}'", GetType().Name);

            try
            {
                WcfClient.ReSubscribe(clientId);
                RaiseOnReSubscribe();
            }
            catch (Exception e)
            {
                Logger.LogException(LogLevel.Error, e, "Caught exception while calling ReSubscribe({0})", clientId);
            }
        }

        /// <summary>
        /// Try to resubscribe to the Wcf Client asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>A Task which represents the asynchronous operation.</returns>
        public Task ReSubscribeAsync(string clientId)
        {
            Logger.Log(LogLevel.Debug, "ReSubscribeAsync '{0}'", GetType().Name);

            try
            {
                var task = WcfClient.ReSubscribeAsync(clientId);

                RaiseOnReSubscribe();

                return task;
            }
            catch (Exception e)
            {
                Logger.LogException(LogLevel.Error, e, "Caught exception while calling ReSubscribeAsync({0})", clientId);
            }

            return null;
        }

        private void RaiseOnReSubscribe()
        {
            OnSubscribed();

            Subscribed?.Invoke(this, new EventArgs());
        }

        private void RaiseConnected()
        {
            Connected?.Invoke(this, new EventArgs());
        }

        private void RaiseDisconnected()
        {
            Diconnected?.Invoke(this, new EventArgs());
        }

        #region Heartbeat

        private Timer _timer;

        /// <summary>
        /// Heartbeat call to the server side.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        public void Heartbeat(string clientId)
        {
            if (WcfClient != null)
            {
                WcfClient.Heartbeat(ClientId);
            }
        }

        /// <summary>
        /// Heartbeat call to the server side asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>A Task which represents the asynchronous operation.</returns>
        public Task HeartbeatAsync(string clientId)
        {
            return WcfClient?.HeartbeatAsync(ClientId);
        }

        private void StartHeartbeat()
        {
            lock (this)
            {
                if(_timer == null)
                {
                    _timer = new Timer(HeartbeatTimerDelay)
                    {
                        AutoReset = false
                    };

                    _timer.Elapsed += HandleHeartbeatTimer;
                }
            }

            _timer.Start();
        }

        private void HandleHeartbeatTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                HeartbeatAsync(ClientId);
            }
            catch (Exception e)
            {
                Logger.LogException(LogLevel.Error, e, "Caught exception while sending Heartbeat");
            }
            finally
            {
                _timer.Start();
            }
        }

        private void StopHeartbeat()
        {
            _timer.Stop();
        }
        #endregion

        /// <summary>
        /// Unsubscribe client from server.
        /// </summary>
        public void UnSubscribe()
        {
            try
            {
                WcfClient.UnSubscribe();
            }
            catch (Exception e)
            {
                Logger.LogException(LogLevel.Error, e, "Caught exception while calling UnSubscribe");
            }
        }

        /// <summary>
        /// Gets the state of the Wcf ClientBase object.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public CommunicationState State => WcfClient?.State ?? CommunicationState.Closed;

        /// <summary>
        /// Gets a value indicating whether [is opened].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is opened]; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpened => State == CommunicationState.Opened;
    }
}
