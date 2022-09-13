// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Moryx.Logging;

namespace Moryx.Communication.Sockets
{
    /// <summary>
    /// Class representing a single port listener
    /// </summary>
    internal class TcpPortListener
    {
        /// <summary>
        /// Create new port listener
        /// </summary>
        public TcpPortListener(IPAddress address, int port)
        {
            Address = address;
            Port = port;
        }

        /// <summary>
        /// IP-Address of the listener
        /// </summary>
        public IPAddress Address { get; }

        /// <summary>
        /// Port of this port listener
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Number of connected listeners on port
        /// </summary>
        public int ConnectionCount => _listeners.Count;

        /// <summary>
        /// Protocol used on this port
        /// </summary>
        private IMessageInterpreter _protocolInterpreter;

        /// <summary>
        /// Some logger instance we steal from a listener
        /// </summary>
        private ILogger _logger;

        /// <summary>
        ///  All listeners on this port
        /// </summary>
        private readonly IList<TcpListenerConnection> _listeners = new List<TcpListenerConnection>();

        /// <summary>
        /// .NET TCP listener instance
        /// </summary>
        private TcpListener _tcpListener;

        /// <summary>
        /// Flag if we still listen on this port
        /// </summary>
        private bool _listening;

        ///
        public void Register(TcpListenerConnection listener)
        {
            lock (_listeners)
            {
                // Steal objects from the first listener
                if (_protocolInterpreter == null)
                    _protocolInterpreter = listener.Validator.Interpreter;
                if (_logger == null)
                    _logger = listener.Logger;

                _listeners.Add(listener);

                StartListening();
            }
        }

        /// 
        public bool TryUnregister(TcpListenerConnection listener)
        {
            lock (_listeners)
            {
                return RemoveListener(listener);
            }
        }

        /// 
        private void StartListening()
        {
            if (_listening)
                return;

            // Start tcp listener and accept clients
            if (_tcpListener == null)
                _tcpListener = new TcpListener(new IPEndPoint(Address, Port));
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(ClientConnected, null);
            _listening = true;
        }

        private void ClientConnected(IAsyncResult ar)
        {
            TcpClient client;
            try
            {
                client = _tcpListener.EndAcceptTcpClient(ar);
            }
            catch (Exception)
            {
                return;
            }

            // Create new transmission and try to assign
            var tcpTransmission = new TcpTransmission(client, _protocolInterpreter, _logger);
            lock (_listeners)
            {
                if (_listeners.Count == 1 && !_listeners[0].ValidateBeforeAssignment)
                {
                    // If we have only one listenere we can assign directly
                    var listener = _listeners[0];
                    RemoveListener(listener);
                    listener.AssignConnection(tcpTransmission);
                }
                else
                {
                    // Wait for the first message to assign the connection
                    tcpTransmission.Disconnected += TransmissionDisconnected;
                    tcpTransmission.Received += InitialReceive;
                }


                // If we have listners without a connection keep accepting clients
                if (_listeners.Count > 0)
                    _tcpListener.BeginAcceptTcpClient(ClientConnected, null);
            }
            tcpTransmission.StartReading();
        }

        private void TransmissionDisconnected(object sender, EventArgs e)
        {
            CleanUp((TcpTransmission)sender);
        }

        private void InitialReceive(object sender, BinaryMessage message)
        {
            TcpListenerConnection listener;
            lock (_listeners)
            {
                listener = _listeners.FirstOrDefault(l => l.Validator.Validate(message));
                RemoveListener(listener);
            }

            var transmission = (TcpTransmission)sender;
            if (listener != null)
            {
                listener.AssignConnection(transmission, message);
            }
            else
            {
                transmission.Dispose();
            }

            CleanUp(transmission);
        }

        private bool RemoveListener(TcpListenerConnection listener)
        {
            var found = _listeners.Remove(listener);
            if (found && _listeners.Count == 0)
            {
                _listening = false;
                _tcpListener.Stop();
            }
            return found;
        }

        private void CleanUp(TcpTransmission transmission)
        {
            transmission.Disconnected -= TransmissionDisconnected;
            transmission.Received -= InitialReceive;
        }
    }
}
