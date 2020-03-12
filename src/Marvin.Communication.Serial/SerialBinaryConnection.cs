// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO.Ports;
using System.Threading.Tasks;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;

namespace Marvin.Communication.Serial
{
    /// <summary>
    /// Implementation of the <see cref="IBinaryConnection"/> interface for
    /// standard serial ports.
    /// </summary>
    [ExpectedConfig(typeof(SerialBinaryConfig))]
    [Plugin(LifeCycle.Transient, typeof(IBinaryConnection), Name = ConnectionName)]
    public class SerialBinaryConnection : IBinaryConnection, ILoggingComponent
    {
        /// <summary>
        /// Unique plugin name for this type
        /// </summary>
        internal const string ConnectionName = "SerialBinaryConnection";

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

        /// <summary>
        /// Create connection instance
        /// </summary>
        public SerialBinaryConnection(IMessageValidator validator)
        {
            _validator = validator;
        }

        ///
        public void Initialize(BinaryConnectionConfig config)
        {
            _config = (SerialBinaryConfig)config;
            _serialPort = SerialPortFactory.FromConfig(_config, Logger);
            _serialPort.DataReceived += OnDataReceived;
        }

        ///
        public void Start()
        {
            _readContext = _validator.Interpreter.CreateContext();
            _serialPort.Open();
            CurrentState = BinaryConnectionState.Connected;
        }

        /// <inheritdoc />
        public void Stop()
        {
            //TODO: Distinguish between IDisposable.Dispose() and Stop()
        }

        ///
        public void Dispose()
        {
            _serialPort.DataReceived -= OnDataReceived;
            _serialPort.Dispose();
            _serialPort = null;

            CurrentState = BinaryConnectionState.Disconnected;
        }

        ///
        public void Reconnect()
        {
        }

        /// <inheritdoc />
        public void Reconnect(int delayMs)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public BinaryConnectionState CurrentState { get; private set; }

#pragma warning disable 67
        /// <inheritdoc />
        public event EventHandler<BinaryConnectionState> NotifyConnectionState;
#pragma warning restore 67

        /// <inheritdoc />
        public void Send(BinaryMessage message)
        {
            // Create bytes from message
            var bytes = _validator.Interpreter.SerializeMessage(message);
            _serialPort.Write(bytes, 0, bytes.Length);
        }

        ///
        public Task SendAsync(BinaryMessage message)
        {
            throw new NotSupportedException();
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

        ///
        public event EventHandler<BinaryMessage> Received;
    }

}
