using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
        private TcpClientConfig _config;

        /// <summary>
        /// Current state of this client connection
        /// </summary>
        private ClientStateBase _state;

        /// <summary>
        /// Current tcp client instance
        /// </summary>
        private TcpClient _tcpClient;

        /// <summary>
        /// Gets the current ConnectionState.
        /// </summary>
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
            _config = (TcpClientConfig)config;

            StateMachine.Initialize(this).With<ClientStateBase>();

            Endpoint = GetIpEndpointFromHost(_config.IpAdress, _config.Port);
        }

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public void Start()
        {
            lock (this)
                _state.Connect();
        }

        /// <inheritdoc />
        public void Stop()
        {
            lock (this)
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
            // ReSharper disable once InconsistentlySynchronizedField
            _state = (ClientStateBase)state;
            NotifyConnectionState?.Invoke(this, _state.Current);
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
            lock (this)
            {
                _state.Disconnect();
                // TODO: Find better delay
                Thread.Sleep(delayMs);
                _state.Connect();
            }
        }

        #region Connection and transmission

        internal IPEndPoint Endpoint { get; set; }

        private TcpTransmission _transmission;

        internal void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.BeginConnect(Endpoint.Address, Endpoint.Port, ConnectionCallback, null);
        }

        internal void RetryConnect()
        {
            if (_config.RetryWaitMs >= 0)
                Thread.Sleep(_config.RetryWaitMs);
            Connect();
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            lock (this)
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
            if (_config.MonitoringIntervalMs > 0)
                _transmission.ConfigureKeepAlive(_config.MonitoringIntervalMs, _config.MonitoringTimeoutMs);
        }

        internal void StopConnect()
        {
            _tcpClient.Close();
            _tcpClient = null;
        }

        private void OnTransmissionException(object sender, Exception e)
        {
            Logger.LogException(LogLevel.Error, e, "Tcp connection encountered an error.");
        }

        private void ConnectionClosed(object sender, EventArgs eventArgs)
        {
            lock (this)
                _state.ConnectionClosed();
        }

        internal void Disconnect()
        {
            // First close the connection and then unregister events
            _transmission.Disconnect();
            _transmission.ExceptionOccured -= OnTransmissionException;
            _transmission.Received -= MessageReceived;
            _transmission.Disconnected -= ConnectionClosed;
            _transmission = null;
        }

        /// <inheritdoc />
        public void Send(BinaryMessage data)
        {
            lock (this)
                _state.Send(data);
        }

        internal void ExecuteSend(BinaryMessage data)
        {
            _transmission.Send(data);
        }

        private void MessageReceived(object sender, BinaryMessage message)
        {
            if (!_validator.Validate(message))
            {
                // If you send us crap we shut you off!
                Reconnect();
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            Received(this, message);
        }

        /// <inheritdoc />
        public event EventHandler<BinaryMessage> Received;

        /// <inheritdoc />
        public event EventHandler<BinaryConnectionState> NotifyConnectionState;

        #endregion
    }
}