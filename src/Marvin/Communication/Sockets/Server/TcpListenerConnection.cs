// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
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
        /// Lock object
        /// </summary>
        private readonly object _stateLock = new object();

        /// <summary>
        /// Open transmission object
        /// </summary>
        private TcpTransmission _transmission;

        /// <summary>
        /// Static TCP server
        /// </summary>
        internal TcpServer Server => TcpServer.Instance;

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
            lock (_stateLock)
                _state.Open();
        }

        /// <inheritdoc />
        public void Stop()
        {
            lock (_stateLock)
                _state.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
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
            lock (_stateLock)
            {
                _state.Reconnect(delayMs);
            }
        }

        void IStateContext.SetState(IState state)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            _state = (ServerStateBase) state;
            try
            {
                NotifyConnectionState?.Invoke(this, _state.CurrentState);
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Fatal, ex, "StateChanged event to {0} ran into an exception!", _state);
            }
        }

        internal void Register()
        {
            Server.Register(this);
        }

        internal void Unregister()
        {
            Server.Unregister(this);
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

        internal void AssignConnection(TcpTransmission transmission, BinaryMessage message = null)
        {
            lock (_stateLock)
                _state.ConnectionAssigned(transmission, message);
        }

        /// <summary>
        /// Assign connection and publish initial message if already available
        /// </summary>
        /// <param name="transmission">Open connection</param>
        internal void ExecuteAssignConnection(TcpTransmission transmission)
        {
            Logger.Log(LogLevel.Info, "Connection established on port {0}", _config.Port);
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

            Unregister();
        }

        private void Disconnected(object sender, EventArgs eventArgs)
        {
            lock (_stateLock)
                _state.ConnectionLost();
        }

        /// <inheritdoc />
        public void Send(BinaryMessage message)
        {
            lock (_stateLock)
                _state.Send(message);
        }

        internal void ExecuteSend(BinaryMessage message)
        {
            _transmission.Send(message);
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
            if (!Validator.Validate(message))
            {
                // If you send us crap we shut you off!
                Reconnect();
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            try
            {
                Received(this, message);
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Fatal, ex, "Received event ran into an exception!");
            }
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
