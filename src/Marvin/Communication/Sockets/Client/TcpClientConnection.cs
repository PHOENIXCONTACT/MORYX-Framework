using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.StateMachines;

namespace Marvin.Communication.Sockets
{
    /// <summary>
    /// Implementation of <see cref="IBinaryConnection"/> as a client connection
    /// </summary>
    [ExpectedConfig(typeof(TcpClientConfig))]
    [Plugin(LifeCycle.Transient, typeof(IBinaryConnection), Name = nameof(TcpClientConnection))]
    public class TcpClientConnection : IBinaryConnection, IStateContext, ILoggingComponent
    {
        #region Dependencies

        /// <summary>
        /// The Logger
        /// </summary>
        [UseChild(nameof(TcpClientConnection))]
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <summary>
        /// Validator for incoming messages
        /// </summary>
        private readonly IMessageValidator _validator;

        /// <summary>
        /// Config for the TCP client
        /// </summary>
        internal TcpClientConfig Config { get; private set; }

        /// <summary>
        /// Current state of this client connection
        /// </summary>
        private ClientStateBase _state;

        /// <summary>
        /// Lock object
        /// </summary>
        private readonly object _stateLock = new object();

        /// <summary>
        /// Current tcp client instance
        /// </summary>
        private TcpClient _tcpClient;

        /// <summary>
        /// Temporary property used for reconnection
        /// </summary>
        internal int ReconnectDelayMs { get; set; }

        /// <summary>
        /// Gets the current ConnectionState.
        /// </summary>
        // ReSharper disable once InconsistentlySynchronizedField
        public BinaryConnectionState CurrentState => _state.Current;

        /// <summary>
        /// Create a new instance of the 
        /// </summary>
        /// <param name="validator"></param>
        public TcpClientConnection(IMessageValidator validator)
        {
            _validator = validator;
        }

        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public void Initialize(BinaryConnectionConfig config)
        {
            Config = (TcpClientConfig)config;

            StateMachine.Initialize(this).With<ClientStateBase>();

            Endpoint = GetIpEndpointFromHost(Config.IpAdress, Config.Port);
        }

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public void Start()
        {
            lock (_stateLock)
                _state.Connect();
        }

        /// <inheritdoc />
        public void Stop()
        {
            lock (_stateLock)
                _state.Disconnect();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <inheritdoc />
        void IStateContext.SetState(IState state)
        {
            // ReSharper disable InconsistentlySynchronizedField
            _state = (ClientStateBase)state;
            try
            {
                NotifyConnectionState?.Invoke(this, _state.Current);
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Fatal, ex, "StateChanged event to {0} ran into an exception!", _state);
            }
            // ReSharper enable InconsistentlySynchronizedField
        }

        /// <summary>
        /// Gets the ip endpoint from host (ip or hostname).
        /// </summary>
        private static IPEndPoint GetIpEndpointFromHost(string hostName, int port)
        {
            var addresses = Dns.GetHostAddresses(hostName);
            if (addresses.Length == 0)
                throw new ArgumentException("Unable to retrieve address from specified host name.", nameof(hostName));

            return new IPEndPoint(addresses[0], port);
        }

        /// <inheritdoc />
        public void Reconnect()
        {
            Reconnect(0);
        }

        /// <inheritdoc />
        public void Reconnect(int delayMs)
        {
            lock(_stateLock)
                _state.Reconnect(delayMs);
        }

        #region Connection and transmission

        internal IPEndPoint Endpoint { get; set; }

        private TcpTransmission _transmission;

        internal void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.BeginConnect(Endpoint.Address, Endpoint.Port, ConnectionCallback, null);
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            lock (_stateLock)
                _state.ConnectionCallback(ar, _tcpClient);
        }

        internal void Connected()
        {
            _transmission = new TcpTransmission(_tcpClient, _validator.Interpreter, Logger.GetChild(string.Empty, typeof(TcpTransmission)));
            _transmission.Disconnected += ConnectionClosed;
            _transmission.ExceptionOccured += OnTransmissionException;
            _transmission.Received += MessageReceived;
            _transmission.StartReading();

            // Configure TCP keep alive
            if (Config.MonitoringIntervalMs > 0)
                _transmission.ConfigureKeepAlive(Config.MonitoringIntervalMs, Config.MonitoringTimeoutMs);
        }

        private void OnTransmissionException(object sender, Exception e)
        {
            Logger.LogException(LogLevel.Error, e, "Tcp connection encountered an error.");
        }

        private void ConnectionClosed(object sender, EventArgs eventArgs)
        {
            lock (_stateLock)
                _state.ConnectionClosed();
        }

        internal void Disconnect()
        {
            var transmission = _transmission;
            _transmission = null;

            var tcpClient = _tcpClient;

            // First close the connection and then unregister events
            transmission.Disconnect();
            transmission.ExceptionOccured -= OnTransmissionException;
            transmission.Received -= MessageReceived;
            transmission.Disconnected -= ConnectionClosed;

            tcpClient.Close();
        }

        internal void CloseClient()
        {
            _tcpClient.Close();
        }

        internal void ScheduleConnectTimer(int delayInMs)
        {
            var reconnectTimer = new Timer(OnReconnectTimer);
            reconnectTimer.Change(delayInMs, Timeout.Infinite);
        }

        private void OnReconnectTimer(object state)
        {
            ((Timer)state).Dispose();

            lock (_stateLock)
                _state.ScheduledConnectTimerElapsed();
        }

        /// <inheritdoc />
        public void Send(BinaryMessage message)
        {
            lock (_stateLock)
                _state.Send(message);
        }

        internal void ExecuteSend(BinaryMessage data)
        {
            _transmission.Send(data);
        }

        /// <inheritdoc />
        public Task SendAsync(BinaryMessage message)
        {
            lock (_stateLock)
                return _state.SendAsync(message);
        }

        internal Task ExecuteSendAsync(BinaryMessage message)
        {
            return _transmission.SendAsync(message);
        }

        private void MessageReceived(object sender, BinaryMessage message)
        {
            if (!_validator.Validate(message))
            {
                // If you send us crap we shut you off!
                Reconnect();
                return;
            }

            try
            {
                // ReSharper disable once PossibleNullReferenceException
                Received(this, message);
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Fatal, ex, "Received event ran into an exception!");
            }
        }

        /// <inheritdoc />
        public event EventHandler<BinaryMessage> Received;

        /// <inheritdoc />
        public event EventHandler<BinaryConnectionState> NotifyConnectionState;

        #endregion
    }
}