// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO.Ports;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.Communication.Serial
{
    /// <summary>
    /// Implementation of the <see cref="IBinaryConnection"/> interface for
    /// standard serial ports.
    /// </summary>
    [ExpectedConfig(typeof(SerialBinaryConfig))]
    [Plugin(LifeCycle.Transient, typeof(IBinaryConnection), Name = nameof(SerialBinaryConnection))]
    public class SerialBinaryConnection : IBinaryConnection, ILoggingComponent
    {
        /// <summary>
        /// Named injected logger instance
        /// </summary>
        [UseChild("SerialConnection")]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Connection configuration
        /// </summary>
        private SerialBinaryConfig _config;

        /// <summary>
        /// Validator and interpreter for incoming messages
        /// </summary>
        private readonly IMessageValidator _validator;

        /// <summary>
        /// Serial port used for the communication
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        /// Read context for incoming data
        /// </summary>
        private IReadContext _readContext;

        private BinaryConnectionState _currentState;

        /// <inheritdoc />
        public BinaryConnectionState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                NotifyConnectionState?.Invoke(this, _currentState);
            }
        }

        /// <summary>
        /// Create connection instance
        /// </summary>
        public SerialBinaryConnection(IMessageValidator validator)
        {
            _validator = validator;
        }

        /// <inheritdoc />
        public void Initialize(BinaryConnectionConfig config)
        {
            _config = (SerialBinaryConfig)config;
            _serialPort = SerialPortFactory.FromConfig(_config, Logger);
            _serialPort.DataReceived += OnDataReceived;
        }

        /// <inheritdoc />
        public void Start()
        {
            _readContext = _validator.Interpreter.CreateContext();
            _serialPort.Open();

            CurrentState = BinaryConnectionState.Connected;
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (_serialPort == null)
                return;

            _serialPort.DataReceived -= OnDataReceived;
            _serialPort.Dispose();
            _serialPort = null;

            CurrentState = BinaryConnectionState.Disconnected;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
        }

        /// <inheritdoc />
        public void Reconnect()
        {
        }

        /// <inheritdoc />
        public void Reconnect(int delayMs)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Send(BinaryMessage message)
        {
            // Create bytes from message
            var bytes = _validator.Interpreter.SerializeMessage(message);
            _serialPort.Write(bytes, 0, bytes.Length);
        }

        /// <inheritdoc />
        public async Task SendAsync(BinaryMessage message)
        {
            // Create bytes from message
            var bytes = _validator.Interpreter.SerializeMessage(message);
            await _serialPort.BaseStream.WriteAsync(bytes, 0, bytes.Length);
            await _serialPort.BaseStream.FlushAsync();
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            int available;
            while ((available = _serialPort.BytesToRead) > 0)
            {
                _serialPort.Read(_readContext.ReadBuffer, _readContext.CurrentIndex, available);
                _validator.Interpreter.ProcessReadBytes(_readContext, available, PublishCompleteMessage);
            }
        }

        private void PublishCompleteMessage(BinaryMessage binaryMessage)
        {
            Received?.Invoke(this, binaryMessage);
        }

        /// <inheritdoc />
        public event EventHandler<BinaryMessage> Received;

        /// <inheritdoc />
        public event EventHandler<BinaryConnectionState> NotifyConnectionState;
    }

}
