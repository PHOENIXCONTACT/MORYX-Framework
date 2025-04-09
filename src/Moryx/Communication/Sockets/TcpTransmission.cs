// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Moryx.Serialization;

namespace Moryx.Communication.Sockets
{
    internal class TcpTransmission : IBinaryTransmission, IDisposable
    {
        private readonly NetworkStream _stream;
        private readonly IMessageInterpreter _interpreter;
        private readonly TcpClient _client;
        private bool _disconnected;

        /// <summary>
        /// Logger for logging - makes sense ;)
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Delegate invoked when connection was lost
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Callback to forward transmission exceptions to connection
        /// </summary>
        public event EventHandler<Exception> ExceptionOccurred;

        /// <summary>
        /// Initialize TcpTransmission
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="interpreter">The interpreter.</param>
        /// <param name="logger">Logger used to write exceptions to log</param>
        public TcpTransmission(TcpClient client, IMessageInterpreter interpreter, ILogger logger)
        {
            _interpreter = interpreter;
            _client = client;
            _stream = client.GetStream();
            _logger = logger;

            // Assign callbacks to make sure the delegate does get collected by the GC
            ReadCallback = ReadComplete;
            SendCallback = SendComplete;
        }

        private void RaiseException(Exception ex)
        {
            if (ExceptionOccurred == null)
                _logger.Log(LogLevel.Error, ex, "TcpTransmission encountered an error");
            else
                ExceptionOccurred(this, ex);
        }

        private void RaiseDisconnected()
        {
            if (Disconnected == null)
                _logger.Log(LogLevel.Warning, "Client disconnected, but listener already removed!");
            else
                Disconnected(this, new EventArgs());
        }

        /// <summary>
        /// Trigger to check if we are still connected
        /// https://stackoverflow.com/a/4667582/6082960
        /// https://stackoverflow.com/a/69497010/6082960
        /// http://tldp.org/HOWTO/TCP-Keepalive-HOWTO/overview.html
        /// </summary>
        public void ConfigureKeepAlive(int interval, int timeout)
        {
            var socket = _client.Client;

#if HAVE_TCP_KEEPALIVE
            // Configure socket using net6 keep alive configuration
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, interval);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, timeout);

            try
            {
                // Try to set the TcpKeeepAliveRetryCount, it is not supported on all windows systems
                // https://learn.microsoft.com/en-us/windows/win32/winsock/ipproto-tcp-socket-options#windows-support-for-ipproto_tcp-options
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 2);
                
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Warning, ex, "TcpKeepAliveRetryCount could not be applied!");
            }

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
#else
            // Keep alive only supported for windows under netstandard2.0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Create config array
                var index = 0;
                var socketConfig = new byte[12]; // 3 * 4 byte
                InlineConverter.Include(1, socketConfig, ref index);
                InlineConverter.Include(interval, socketConfig, ref index);
                InlineConverter.Include(timeout, socketConfig, ref index);
                socket.IOControl(IOControlCode.KeepAliveValues, socketConfig, null);
            }
#endif
        }

        #region Send

        /// <inheritdoc />
        public void Send(BinaryMessage message)
        {
            var bytes = _interpreter.SerializeMessage(message);
            _stream.BeginWrite(bytes, 0, bytes.Length, SendCallback, null);
        }

        /// <summary>
        /// Reference to the callback to avoid access violation exceptions
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/f5e90354-6ef9-410e-a53b-3f7a3a0dd625/unhandled-accessviolationexception-in-external-code-systemthreadingiocompletioncallback?forum=csharpgeneral
        /// </summary>
        private AsyncCallback SendCallback { get; }

        /// <summary>
        /// Callback. Is called when the sending was completed.
        /// </summary>
        private void SendComplete(IAsyncResult result)
        {
            try
            {
                _stream.EndWrite(result);
            }
            catch (Exception ex)
            {
                Disconnect(ex);
            }
        }

        /// <inheritdoc />
        public async Task SendAsync(BinaryMessage message)
        {
            var bytes = _interpreter.SerializeMessage(message);

            try
            {
                await Task.Factory.FromAsync(_stream.BeginWrite, _stream.EndWrite, bytes, 0, bytes.Length, null);
                await _stream.FlushAsync();
            }
            catch (Exception ex)
            {
                Disconnect(ex);
            }
        }

        #endregion

        #region Receive

        /// <summary>
        /// Start reading shit
        /// </summary>
        public void StartReading()
        {
            var context = _interpreter.CreateContext();
            BeginRead(context);
        }

        private void BeginRead(IReadContext transmission)
        {
            try
            {
                if (_disconnected)
                    return;

                _stream.BeginRead(transmission.ReadBuffer, transmission.CurrentIndex, transmission.ReadSize, ReadCallback, transmission);
            }
            catch (ObjectDisposedException)
            {
                Disconnect();
            }
            catch (IOException)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Reference to the callback to avoid access violation exceptions
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/f5e90354-6ef9-410e-a53b-3f7a3a0dd625/unhandled-accessviolationexception-in-external-code-systemthreadingiocompletioncallback?forum=csharpgeneral
        /// </summary>
        private AsyncCallback ReadCallback { get; }

        private void ReadComplete(IAsyncResult ar)
        {
            int read;

            if (_disconnected)
                return;

            try
            {
                read = _stream.EndRead(ar);
            }
            catch (Exception ex)
            {
                Disconnect(ex);
                return;
            }

            var transmission = (IReadContext)ar.AsyncState;
            ByteReadResult result;

            if (read > 0)
            {
                result = _interpreter.ProcessReadBytes(transmission, read, PublishMessage);
            }
            else
            {
                Disconnect();
                return;
            }

            // Error in stream - send last will and close transmission
            if (result == ByteReadResult.Failure)
            {
                byte[] lastWill;
                if (_interpreter.ErrorResponse(transmission, out lastWill))
                    _stream.Write(lastWill, 0, lastWill.Length);

                Disconnect(new InvalidHeaderException("Header invalid or no matching validator found!"));
                return;
            }

            BeginRead(transmission);
        }

        private void PublishMessage(BinaryMessage message)
        {
            if (Received == null && _disconnected) // Still feels like a hack
                _logger.Log(LogLevel.Error, "Connection already closed, but a final message was received and can not be published!");
            else
                Received.Invoke(this, message);
        }

        ///
        public event EventHandler<BinaryMessage> Received;

        #endregion

        private void Disconnect(Exception ex)
        {
            RaiseException(ex);
            Disconnect();
        }

        internal void Disconnect()
        {
            lock (this)
            {
                if (_disconnected)
                    return;
                _disconnected = true;
            }

            _stream.Dispose();
            _client.Close();
            RaiseDisconnected();
        }

        ///
        public void Dispose()
        {
            Disconnect();
        }
    }
}
