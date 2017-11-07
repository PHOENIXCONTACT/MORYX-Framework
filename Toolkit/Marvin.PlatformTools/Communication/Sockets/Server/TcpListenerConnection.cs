using System;
using System.Threading;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules.ModulePlugins;
using Marvin.StateMachines;

namespace Marvin.Communication.Sockets
{
    /// <summary>
    /// Implementation of <see cref="IBinaryConnection"/> as a server connection
    /// </summary>
    [ExpectedConfig(typeof(TcpListenerConfig))]
    [Plugin(LifeCycle.Transient, typeof(IBinaryConnection), Name = nameof(TcpListenerConnection))]
    public class TcpListenerConnection : IBinaryConnection, IStateContext, ILoggingComponent, IDisposable
    {
        #region Dependencies

        /// <inheritdoc />
        [UseChild]
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <summary>
        /// Configuration of this connection
        /// </summary>
        private TcpListenerConfig _config;

        /// <summary>
        /// Current state of the connection
        /// </summary>
        private ServerStateBase _state;

        /// <summary>
        /// Open transmission object
        /// </summary>
        private TcpTransmission _transmission;

        /// <summary>
        /// Static TCP server
        /// </summary>
        internal ITcpServer Server => TcpServer.Instance;

        /// <summary>
        /// Port this is instance is listening on
        /// </summary>
        internal int Port => _config.Port;

        /// <summary>
        /// Flag if incoming connections always need to validate the first message before assigning them
        /// </summary>
        internal bool ValidateBeforeAssignment => _config.ValidateBeforeAssignment;

        /// <summary>
        /// Validator used for incoming message and listener selection
        /// </summary>
        internal IMessageValidator Validator { get; }

        /// <summary>
        /// Gets the current ConnectionState.
        /// </summary>
        public BinaryConnectionState CurrentState => _state.CurrentState;

        /// <summary>
        /// Create listener connection
        /// </summary>
        /// <param name="validator">Validator for this listener</param>
        public TcpListenerConnection(IMessageValidator validator)
        {
            Validator = validator;
        }

        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public void Initialize(BinaryConnectionConfig config)
        {
            _config = (TcpListenerConfig)config;

            StateMachine.Initialize(this).With<ServerStateBase>();
        }

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public void Start()
        {
            lock (this)
                _state.Open();
        }

        /// <inheritdoc />
        public void Stop()
        {
            //TODO: Distinguish between IDisposable.Dispose() and Stop()
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (this)
                _state.Close();
        }

        /// <summary>
        /// In case we think our connection is broken we can enforce a reconnect here
        /// </summary>
        public void Reconnect()
        {
            Reconnect(0);
        }

        /// <summary>
        /// In case we think our connection is broken we can enforce a reconnect here after giving the device time to reset
        /// </summary>
        public void Reconnect(int delayMs)
        {
            lock (this)
            {
                _state.Close();
                // TODO: Find better delay
                Thread.Sleep(delayMs);
                _state.Open();
            }
        }

        void IStateContext.SetState(IState state)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            _state = (ServerStateBase) state;
            NotifyConnectionState?.Invoke(this, _state.CurrentState);
        }

        internal void Register()
        {
            Server.Register(this);
        }

        internal void Unregister()
        {
            Server.Unregister(this);
        }

        internal void AssignConnection(TcpTransmission transmission, BinaryMessage message = null)
        {
            lock (this)
                _state.ConnectionAssigned(transmission, message);
        }

        /// <summary>
        /// Assign connection and publish initial message if already available
        /// </summary>
        /// <param name="transmission">Open connection</param>
        internal void ExecuteAssignConnection(TcpTransmission transmission)
        {
            Logger.LogEntry(LogLevel.Info, "Connection established on port {0}", _config.Port);
            _transmission = transmission;
            _transmission.Disconnected += Disconnected;
            _transmission.Received += MessageReceived;

            // Configure keep alive on tcp
            if (_config.MonitoringIntervalMs > 0)
                _transmission.ConfigureKeepAlive(_config.MonitoringIntervalMs, _config.MonitoringTimeoutMs);
        }

        /// <summary>
        /// The initial message, usually a logon that was used to determine this listener 
        /// </summary>
        internal void PublishInitialMessage(BinaryMessage message)
        {
            MessageReceived(this, message);
        }

        /// <summary>
        /// Clean up transmission object and optionally close it first
        /// </summary>
        internal void CleanupTransmission()
        {
            // First close connection if specified
            _transmission.Disconnect();
            _transmission.Received -= MessageReceived;
            _transmission.Disconnected -= Disconnected;
            _transmission = null;
        }

        private void Disconnected(object sender, EventArgs eventArgs)
        {
            lock (this)
                _state.ConnectionLost();
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
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
            // ReSharper disable once PossibleNullReferenceException
            Received(this, message);
        }

        /// <summary>
        /// Event raised when message was received
        /// </summary>
        public event EventHandler<BinaryMessage> Received;

        /// <summary>
        /// Event raised when connection state changes
        /// </summary>
        public event EventHandler<BinaryConnectionState> NotifyConnectionState;
    }
}