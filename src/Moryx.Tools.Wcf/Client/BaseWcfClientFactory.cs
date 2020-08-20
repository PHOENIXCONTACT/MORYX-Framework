// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Moryx.Communication;
using Moryx.Logging;
using Moryx.Threading;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Abstract base class for WCF client factories.
    /// </summary>
    public abstract class BaseWcfClientFactory : IWcfClientFactory
    {
        #region Dependencies

        /// <summary>Injected property</summary>
        public IModuleLogger Logger { get; set; }

        #endregion

        #region Fields and Properties

        // Monitoring
        private long _lastClientId;
        private readonly List<MonitoredClient> _monitoredClients = new List<MonitoredClient>();
        private readonly ICollection<WcfClientInfo> _clientInfos = new List<WcfClientInfo>();

        // Initialization Configuraion
        private IThreadContext _threadContext;
        private IProxyConfig _proxyConfig;
        private IWcfClientFactoryConfig _factoryConfig;
        private Timer _monitorTimer;

        // Version Service - internal for the integrationtests
        internal IVersionServiceManager VersionService { get; set; }

        ///
        public IEnumerable<WcfClientInfo> ClientInfos => _clientInfos;

        ///
        public string ClientId => _factoryConfig.ClientId;

        #endregion

        #region Initialization and Disposing

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWcfClientFactory"/> class.
        /// </summary>
        protected BaseWcfClientFactory()
        {
            VersionService = new VersionServiceManager();
        }

        /// <summary>
        /// Initializes this factory.
        /// </summary>
        /// <param name="factoryConfig">The configuration of this factory.</param>
        /// <param name="proxyConfig">An optional proxy configuration.</param>
        /// <param name="threadContext">For WPF applications, an instance of WpfThreadContext should be passed here.
        /// For console applications or Windows services an instance of SimpleThreadContext should be used instead.</param>
        protected void Initialize(IWcfClientFactoryConfig factoryConfig, IProxyConfig proxyConfig, IThreadContext threadContext)
        {
            _factoryConfig = factoryConfig;
            _proxyConfig  = proxyConfig;
            _threadContext = threadContext;
        }

        /// <summary>
        /// Connects to version service and starts the monitor thread
        /// </summary>
        private void StartOnDemand()
        {
            if (!VersionService.IsInitialized)
            {
                VersionService.Initialize(_proxyConfig, _factoryConfig.Host, _factoryConfig.Port);
            }

            if (_monitorTimer == null)
            {
                _monitorTimer = new Timer(new NonStackingTimerCallback(state =>
                {
                    // Main operation which will iterate through the current monitored clients
                    ConnectOfflineClients();
                    CloseDestroyedClients();
                }), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Logger.Log(LogLevel.Info, "Disposing WcfClientFactory!");

            if (_monitorTimer != null)
            {
                var waitEvent = new ManualResetEvent(false);
                _monitorTimer.Dispose(waitEvent);
                waitEvent.WaitOne();

                _monitorTimer = null;
            }

            var connectedClients = GetMonitoredClients(c => c.State == InternalConnectionState.Success);

            // Inform server of shutdown for all clients
            foreach (var client in connectedClients)
            {
                InvokeDestroyClient(client);
            }
        }

        #endregion

        #region IWcfClientFactory

        #region Create with Config

        ///
        public long Create<T, TK>(IClientConfig config, Action<ConnectionState, T> callback)
            where T : ClientBase<TK>
            where TK : class
        {
            return Create<T, TK>(config, null, callback);
        }

        ///
        public long Create<T, TK>(IClientConfig config, object callbackService, Action<ConnectionState, T> callback)
            where T : ClientBase<TK>
            where TK : class
        {
            return Create<T, TK>(config, callbackService, callback, null);
        }

        ///
        public long Create<T, TK>(IClientConfig config, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class
        {
            return Create<T, TK>(config, null, callback, binding);
        }

        ///
        public long Create<T, TK>(IClientConfig config, object callbackService, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrEmpty(config.Endpoint))
                throw new ArgumentException("Endpoint not set.", nameof(config));

            if (config.Port <= 0 || config.Port >= 65536)
                throw new ArgumentException($"Invalid port number {config.Port}.", nameof(config));

            return AddClientToMonitor<T, TK>(config, callbackService, callback, binding);
        }

        #endregion

        #region Create without config

        ///
        public long Create<T, TK>(string clientVersion, string minServerVersion, Action<ConnectionState, T> callback)
            where T : ClientBase<TK>
            where TK : class
        {
            return Create<T, TK>(clientVersion, minServerVersion, null, callback);
        }

        ///
        public long Create<T, TK>(string clientVersion, string minServerVersion, object callbackService, Action<ConnectionState, T> callback)
            where T : ClientBase<TK>
            where TK : class
        {
            return Create<T, TK>(clientVersion, minServerVersion, callbackService, callback, null);
        }

        ///
        public long Create<T, TK>(string clientVersion, string minServerVersion, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class
        {
            return Create<T, TK>(clientVersion, minServerVersion, null, callback, binding);
        }

        ///
        public long Create<T, TK>(string clientVersion, string minServerVersion, object callbackService, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class
        {
            if (string.IsNullOrEmpty(clientVersion))
                throw new ArgumentNullException(nameof(clientVersion));

            if (string.IsNullOrEmpty(minServerVersion))
                throw new ArgumentNullException(nameof(minServerVersion));

            var clientConfig = new ClientVersionConfig(clientVersion, minServerVersion);

            return AddClientToMonitor<T, TK>(clientConfig, callbackService, callback, binding);
        }

        #endregion

        ///
        public void Destroy(long clientId)
        {
            var client = GetMonitoredClients(c => c.ClientInfo.Id == clientId).FirstOrDefault();
            if (client == null)
            {
                Logger.Log(LogLevel.Error, "Can't destroy unknown WCF client {0}", clientId);
                throw new InvalidOperationException($"Client with id {clientId} was not found");
            }

            Logger.Log(LogLevel.Info, "Destroying WCF client {0}", clientId);
            client.Destroy = true;
        }

        ///
        public WcfClientInfo GetClientById(long clientId)
        {
            var clientInfo = _clientInfos.FirstOrDefault(c => c.Id == clientId);
            if (clientInfo == null)
                throw new InvalidOperationException($"Client with id {clientId} was not found");

            return clientInfo;
        }

        #endregion

        /// <summary>
        /// Adds a new client to the monitoring quene.
        /// </summary>
        private long AddClientToMonitor<T, TK>(IClientVersionConfig config, object callbackService, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            // ReSharper disable ConvertToLambdaExpression

            StartOnDemand();

            var serviceName = typeof(TK).Name;
            var clientType = typeof(T);

            // Add client to collection
            var request = new MonitoredClient
            {
                Binding = binding,
                ClientType = clientType,
                ServiceName = serviceName,
                CallbackService = callbackService,
                ClientInfo = new WcfClientInfo(CreateClientId(), serviceName, config),
            };

            //Wraps the client callback in safe delegate to avoid invoke exceptions
            request.ConnectionCallback = delegate(ConnectionState state, ICommunicationObject client)
            {
                try
                {
                    callback(state, (T)client);
                }
                catch (Exception e)
                {
                    Logger.LogException(LogLevel.Error, e, "Factory callback threw exception");
                }
            };

            //Set delegate to get the inner channel of the wcf client
            request.GetInnerChannel = client => ((T) client).InnerChannel;

            //Set delegate to get the ClientCredentials of the wcf client
            request.GetClientCredentials = client => ((T) client).ClientCredentials;

            //Set delegate to add additional endpoint behaviors
            request.AddEndpointBehavior = (client, behavior) => ((T) client).Endpoint.Behaviors.Add(behavior);

            if (string.IsNullOrEmpty(config.MinServerVersion))
            {
                Logger.Log(LogLevel.Info, "Creating WCF client {0} for '{1}' without version check", request.ClientInfo.Id, serviceName);
            }
            else
            {
                Logger.Log(LogLevel.Info, "Creating WCF client {0} for '{1}', client version {2}, min. service version {3}",
                    request.ClientInfo.Id, serviceName, config.ClientVersion, config.MinServerVersion);
            }

            var clientConfig = config as IClientConfig;
            if (clientConfig != null)
            {
                request.Config = clientConfig;
                request.ClientInfo.Uri = CreateEndpointAddress(request.ServiceConfiguration, request.Config).Uri.ToString();
            }

            lock (_monitoredClients)
                _monitoredClients.Add(request);

            _clientInfos.Add(request.ClientInfo);

            RaiseClientInfoChanged(request.ClientInfo);

            return request.ClientInfo.Id;
        }

        private void ConnectOfflineClients()
        {
            var clients = GetMonitoredClients(CheckIsClientOffline);
            foreach (var client in clients)
            {
                RecieveVersion(client);
            }
        }

        private void CloseDestroyedClients()
        {
            var clients = GetMonitoredClients(c => c.Destroy);

            foreach (var clientToDestroy in clients)
            {
                try
                {
                    InvokeDestroyClient(clientToDestroy);
                }
                catch (Exception e)
                {
                    Logger.LogException(LogLevel.Error, e,
                        "Caught unexpected exception while trying to destroy client for service '{0}'",
                        clientToDestroy.ServiceName);
                }

                lock (_monitoredClients)
                {
                    _monitoredClients.Remove(clientToDestroy);
                }
            }
        }

        /// <summary>
        /// Checks if the client is currently connected
        /// </summary>
        private static bool CheckIsClientOffline(MonitoredClient client)
        {
            if (client.Destroy)
            {
                return false;
            }

            switch (client.State)
            {
                case InternalConnectionState.New:
                case InternalConnectionState.FailedTry:
                case InternalConnectionState.ConnectionLost:
                case InternalConnectionState.Closed:
                case InternalConnectionState.VersionMissmatch:
                case InternalConnectionState.VersionMatch:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Sets the state of the <see cref="WcfClientInfo"/> and increases the tries
        /// </summary>
        private void SetClientInfoState(WcfClientInfo clientInfo, InternalConnectionState internalState)
        {
            var newState = StateTransformer.FromInternal(internalState, clientInfo.State);
            if (clientInfo.State == newState)
            {
                clientInfo.Tries++;
            }
            else
            {
                clientInfo.State = newState;
                clientInfo.Tries = 1;
            }

            RaiseClientInfoChanged(clientInfo);
        }

        /// <summary>
        /// Matches the version of the given client and writes it to the client info.
        /// </summary>
        private void RecieveVersion(MonitoredClient client)
        {
            try
            {
                if (client.Config != null)
                {
                    RecievieVersionWithConfig(client);
                }
                else
                {
                    RecieveVersionWithoutConfig(client);
                }
            }
            catch (Exception e)
            {
                HandleConnectionFailure(client, e.Message);
            }
        }

        /// <summary>
        /// Recievies the version of the client from the versionservice by a given config
        /// </summary>
        private void RecievieVersionWithConfig(MonitoredClient client)
        {
            if (string.IsNullOrEmpty(client.Config.MinServerVersion))
            {
                // Don't do any version check.
                HandleConnectionState(client, InternalConnectionState.VersionMatch);
            }
            else
            {
                Logger.Log(LogLevel.Debug, "Trying to get version info for service '{0}'", client.ServiceName);

                var version = VersionService.GetServerVersion(client.Config.Endpoint);
                if (string.IsNullOrEmpty(version))
                {
                    HandleConnectionFailure(client, "Empty answer.");
                    return;
                }

                // Finally reached the server
                Logger.Log(LogLevel.Debug, "Got version '{0}' info for service '{1}'", version, client.ServiceName);

                client.ClientInfo.ServerVersion = version;

                var versionMatch = VersionService.Match(client.Config, version)
                    ? InternalConnectionState.VersionMatch
                    : InternalConnectionState.VersionMissmatch;

                HandleConnectionState(client, versionMatch);
            }
        }

        /// <summary>
        /// Recieves the version without a given configuration. The configuration will be loaded from
        /// the version service
        /// </summary>
        private void RecieveVersionWithoutConfig(MonitoredClient client)
        {
            Logger.Log(LogLevel.Debug, "Trying to get service configuration for service '{0}'", client.ServiceName);

            client.ServiceConfiguration = VersionService.GetServiceConfiguration(client.ServiceName);

            if (client.ServiceConfiguration == null)
            {
                HandleConnectionFailure(client, "Empty answer.");

                return;
            }

            // Finally reached the server
            Logger.Log(LogLevel.Debug, "Got service configuration for service '{0}'", client.ServiceName);

            client.ClientInfo.ServerVersion = client.ServiceConfiguration.ServerVersion;
            client.ClientInfo.MinClientVersion = client.ServiceConfiguration.MinClientVersion;
            client.ClientInfo.Uri = client.ServiceConfiguration.ServiceUrl;

            var versionMatch = VersionService.Match(client.ClientInfo, client.ServiceConfiguration)
                    ? InternalConnectionState.VersionMatch
                    : InternalConnectionState.VersionMissmatch;

            HandleConnectionState(client, versionMatch);
        }

        /// <summary>
        /// Handles the state of the connection.
        /// </summary>
        private void HandleConnectionState(MonitoredClient client, InternalConnectionState newState)
        {
            var oldState = client.State;

            client.State = newState;
            SetClientInfoState(client.ClientInfo, newState);

            // Only report state changes if the state has changed
            if (newState == oldState)
                return;

            if ((oldState == InternalConnectionState.New && newState == InternalConnectionState.FailedTry)
                || (newState == InternalConnectionState.ConnectionLost) || (newState == InternalConnectionState.Closed))
            {
                RaiseClientDisconnected(client.ServiceName);
            }

            if (!client.Destroy)
            {
                _threadContext.Invoke(() => InvokeCreateClient(client));
            }
        }

        /// <summary>
        /// Handles a connection failure and throws a failed try.
        /// </summary>
        private void HandleConnectionFailure(MonitoredClient client, string message)
        {
            Logger.Log(LogLevel.Debug, "Can't get version info for service '{0}': {1}", client.ServiceName, message);

            client.ClientInfo.ServerVersion = WcfClientInfo.Unknown;
            client.ClientInfo.MinClientVersion = WcfClientInfo.Unknown;
            client.ClientInfo.Uri = WcfClientInfo.Unknown;

            HandleConnectionState(client, InternalConnectionState.FailedTry);
        }

        /// <summary>
        /// Invokes the creation of the client. Will instanciate the client object and raises the
        /// success event.
        /// </summary>
        /// <param name="client">The client.</param>
        private void InvokeCreateClient(MonitoredClient client)
        {
            try
            {
                if (client.State == InternalConnectionState.VersionMatch)
                {
                    client.Instance = InstanciateClientObject(client);
                    client.State = InternalConnectionState.Success;
                    SetClientInfoState(client.ClientInfo, client.State);

                    client.ConnectionCallback(client.ClientInfo.State, client.Instance);

                    RaiseClientConnected(client.ServiceName);
                }
                else
                {
                    client.ConnectionCallback(client.ClientInfo.State, client.Instance);
                }

                return;
            }
            catch (CommunicationException e)
            {
                Logger.LogException(LogLevel.Error, e, "Can't open connection to {0}");
            }
            catch (TimeoutException e)
            {
                Logger.LogException(LogLevel.Error, e, "Can't open connection to {0}");
            }
            catch (Exception e)
            {
                Logger.LogException(LogLevel.Error, e, "Caught unexpected exception");
            }

            client.State = InternalConnectionState.FailedTry;
            SetClientInfoState(client.ClientInfo, client.State);
        }

        #region External Events

        ///
        public event EventHandler AllClientsConnected;

        ///
        public event EventHandler<WcfClientInfo> ClientInfoChanged;
        private void RaiseClientInfoChanged(WcfClientInfo clientInfo)
        {
            Logger.Log(LogLevel.Debug, "Firing ClientConnected");

            if (ClientInfoChanged != null && clientInfo != null)
                ClientInfoChanged(this, clientInfo);
        }

        ///
        public event EventHandler<string> ClientDisconnected;
        private void RaiseClientDisconnected(string endpoint)
        {
            Logger.Log(LogLevel.Debug, "Firing ClientDisconnected for endpoint '{0}'", endpoint);
            ClientDisconnected?.Invoke(this, endpoint);
        }

        ///
        public event EventHandler<string> ClientConnected;
        private void RaiseClientConnected(string endpoint)
        {
            Logger.Log(LogLevel.Debug, "Firing ClientConnected for endpoint '{0}'", endpoint);
            ClientConnected?.Invoke(this, endpoint);

            //If now all clients connected, we can raise the event
            var clients = GetMonitoredClients();
            if (clients.All(client => client.State == InternalConnectionState.Success))
            {
                Logger.Log(LogLevel.Debug, "Firing AllClientsConnected");
                AllClientsConnected?.Invoke(this, new EventArgs());
            }
        }

        #endregion

        #region Event Handler

        private void HandleContextEvent(object sender, DisconnectReason reason)
        {
            var client = GetMonitoredClients(c => c.CallbackContext == sender).FirstOrDefault();
            HandleDisconnect(client, reason);
        }

        private void HandleClientEvent(object sender, DisconnectReason reason)
        {
            var client = GetMonitoredClients(c => c.InnerChannel == sender).FirstOrDefault();
            HandleDisconnect(client, reason);
        }

        private void HandleDisconnect(MonitoredClient client, DisconnectReason reason)
        {
            if (client == null)
            {
                Logger.Log(LogLevel.Warning, "Connection of unknown client/callback {0}", reason);
                return;
            }

            Logger.Log(LogLevel.Warning, "Connection/Callback to '{0}' {1}", client.ServiceName, reason);

            // State of the client. If the client was destroyed, the internal connection state will be set to closed.
            // If the client was not destroryed, the connection was lost.
            var clientState = client.Destroy ? InternalConnectionState.Closed : InternalConnectionState.ConnectionLost;

            HandleConnectionState(client, clientState);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Savely will recieve the monitored clients from the current list.
        /// Locks the monitored list and returns the clients
        /// </summary>
        private IEnumerable<MonitoredClient> GetMonitoredClients(Func<MonitoredClient, bool> expression = null)
        {
            List<MonitoredClient> clients;
            lock (_monitoredClients)
            {
                clients = expression == null ? _monitoredClients.ToList() : _monitoredClients.Where(expression).ToList();
            }

            return clients;
        }

        /// <summary>
        /// Instanciates the client object.
        /// </summary>
        private ICommunicationObject InstanciateClientObject(MonitoredClient monitoredClient)
        {
            var clientParams = new List<object>();

            var serviceConfiguration = monitoredClient.ServiceConfiguration;
            var monitoredClientConfig = monitoredClient.Config;
            var bindingType = serviceConfiguration?.BindingType ?? monitoredClientConfig.BindingType;

            var requiresAuthentication = (serviceConfiguration != null && serviceConfiguration.RequiresAuthentication)
                                         || (monitoredClientConfig != null && monitoredClientConfig.RequiresAuthentification);

            // Add callback context if the service is an net.tcp service
            if (bindingType == BindingType.NetTcp && monitoredClient.CallbackService != null)
            {
                monitoredClient.CallbackContext = new InstanceContext(monitoredClient.CallbackService);

                clientParams.Add(monitoredClient.CallbackContext);

                // Called if the callback context was closed
                monitoredClient.CallbackContext.Closed += delegate(object sender, EventArgs args)
                {
                    HandleContextEvent(sender, DisconnectReason.Closed);
                };

                // Called if the callback context was faulted
                monitoredClient.CallbackContext.Faulted += delegate(object sender, EventArgs args)
                {
                    HandleContextEvent(sender, DisconnectReason.Faulted);
                };
            }

            var binding = monitoredClient.Binding ?? BindingFactory.CreateDefault(bindingType, requiresAuthentication, _proxyConfig);
            var endpointAddress = CreateEndpointAddress(serviceConfiguration, monitoredClientConfig);

            clientParams.Add(binding);
            clientParams.Add(endpointAddress);

            var clientObj = Activator.CreateInstance(monitoredClient.ClientType, clientParams.ToArray()) as ICommunicationObject;

            // Add behaviors
            monitoredClient.AddEndpointBehavior(clientObj, new CultureBehavior());

            // Set credentials
            if (monitoredClientConfig != null)
            {
                var credentials = monitoredClient.GetClientCredentials(clientObj);

                credentials.UserName.UserName = monitoredClientConfig.UserName;
                credentials.UserName.Password = monitoredClientConfig.Password;
            }

            // ReSharper disable once PossibleNullReferenceException
            clientObj.Open();

            // Register to communication faulted event for reconnect
            clientObj.Faulted += delegate(object sender, EventArgs args)
            {
                HandleClientEvent(sender, DisconnectReason.Faulted);
            };

            // Register to communication closed event for reconnect
            clientObj.Closed += delegate(object sender, EventArgs args)
            {
                HandleClientEvent(sender, DisconnectReason.Closed);
            };

            monitoredClient.InnerChannel = monitoredClient.GetInnerChannel(clientObj);

            return clientObj;
        }

        /// <summary>
        /// Creates the endpoint address out of the service configuration.
        /// If the service configuration is set to null, the endpoint will be created with the service url
        /// </summary>
        private EndpointAddress CreateEndpointAddress(ServiceConfiguration serviceConfig, IClientConfig clientConfig)
        {
            if (serviceConfig != null)
                return new EndpointAddress(serviceConfig.ServiceUrl);

            //Set binding type for the uri
            var bindingType = clientConfig.BindingType == BindingType.NetTcp ? "net.tcp" : "http";

            //Set host. Use host from configuration, and if not set, than use host from factory config
            var host = string.IsNullOrEmpty(clientConfig.Host) ? _factoryConfig.Host : clientConfig.Host;

            //Create uri like net.tcp://localhost:80/WebService
            var uri = $"{bindingType}://{host}:{clientConfig.Port}/{clientConfig.Endpoint}";

            return new EndpointAddress(uri);
        }

        #endregion

        /// <summary>
        /// Invokes the destroy of the client.
        /// Will kill the instance and close the connection
        /// </summary>
        private void InvokeDestroyClient(MonitoredClient client)
        {
            // Invoke callback with closing state to enable safe communication shutdown
            HandleConnectionState(client, InternalConnectionState.Closing);

            lock (client)
            {
                if (client.Instance == null)
                    return;

                try
                {
                    client.Instance.Close();
                }
                catch
                {
                    client.Instance.Abort();
                }
            }
        }

        /// <summary>
        /// Creates a new client identifier for new clients. Statically increases a long value.
        /// </summary>
        private long CreateClientId()
        {
            lock (_monitoredClients)
                return ++_lastClientId;
        }
    }
}
