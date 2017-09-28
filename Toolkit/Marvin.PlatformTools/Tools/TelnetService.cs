using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Marvin.Logging;
using Marvin.Testing;

namespace Marvin.Tools
{
    /// <summary>
    /// A simple Telnet service implementation
    /// </summary>
    [OpenCoverIgnore]
    public class TelnetService
    {
        private class ReadLineEventArgs : EventArgs
        {
            public string Line { get; set; }

            public ReadLineEventArgs(string line)
            {
                Line = line;
            }
        }

        private delegate void ReadLineEventHandler(object sender, ReadLineEventArgs e);

        private TcpListener _listener;
        private WorkerThread _listenerThread;
        private readonly IList<Client> _clients = new List<Client>();
        private int _tcpPort;

        private readonly ConcurrentQueue<string> _lineQueue = new ConcurrentQueue<string>();

        private IModuleLogger _logger;

        /// <summary>
        /// Initializes the Telnet service.
        /// </summary>
        /// <param name="tcpPort">The TCP port to listen on.</param>
        /// <param name="logger">A logger to be used for log messages.</param>
        public void Initialize(int tcpPort, IModuleLogger logger)
        {
            _tcpPort = tcpPort;
            _logger = logger;
        }

        /// <summary>
        /// Starts the Telnet service.
        /// </summary>
        public void Start()
        {
            _logger.LogEntry(LogLevel.Debug, "Creating listener socket on port {0}", _tcpPort);

            _listener = new TcpListener(IPAddress.Any, _tcpPort);
            _listener.Start(5);

            _listenerThread = new WorkerThread("TelnetService.ListenerThread", ListenerDelegate, 1, _logger);
            _listenerThread.Start();
        }

        /// <summary>
        /// Stops the Telnet service
        /// </summary>
        public void Stop()
        {
            _listenerThread.Stop();
            _listener.Stop();

            foreach (Client client in _clients)
            {
                client.Close();
            }

            _clients.Clear();
        }

        /// <summary>
        /// Waits for a line of text received from one of the clients.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            string result = null;

            while (_listenerThread.IsAlive && ! _lineQueue.TryDequeue(out result))
            {
                Thread.Sleep(100);
            }

            return result;
        }

        private void ListenerDelegate()
        {
            TcpClient tcpClient = _listener.AcceptTcpClient();

            _logger.LogEntry(LogLevel.Debug, "New client connected.");

            Client client = new Client(tcpClient, _logger);
            client.LineCompletedEvent += (sender, args) => LineCompletedEvent(args);

            ThreadPool.QueueUserWorkItem(client.Run);

            _clients.Add(client);
        }

        private void LineCompletedEvent(ReadLineEventArgs args)
        {
            _lineQueue.Enqueue(args.Line);
        }

        private class Client : IDisposable
        {
            private NetworkStream _stream;
            private IModuleLogger _logger;
            private bool _shallRun;

            public ReadLineEventHandler LineCompletedEvent { get; set; }

            public Client(TcpClient tcpClient, IModuleLogger logger)
            {
                _stream = tcpClient.GetStream();
                _logger = logger;
            }

            public void Close()
            {
                _shallRun = false;

                try
                {
                    if (_stream != null)
                    {
                        _stream.Close();
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // Do nothing
                }
            }

            public void Run(object state)
            {
                try
                {
                    ReadLine();
                }
                catch (Exception e)
                {
                    _logger.LogException(LogLevel.Error, e, "Caught unexpected exception");
                }
                finally
                {
                    Close();
                }
            }

            private void ReadLine()
            {
                String currentLine = String.Empty;

                _shallRun = true;

                while (_shallRun)
                {
                    int i = _stream.ReadByte();

                    if (i < 0)
                    {
                        break;
                    }

                    if (i > 255)  // Limit to ASCII characters
                    {
                        continue;
                    }

                    char c = (char)i;

                    if (Char.IsControl(c))
                    {
                        currentLine = currentLine.Trim();

                        if (currentLine.Length > 0)
                        {
                            _logger.LogEntry(LogLevel.Debug, "Received line: '{0}'", currentLine);

                            if (LineCompletedEvent != null)
                            {
                                LineCompletedEvent(this, new ReadLineEventArgs(currentLine));
                            }

                            currentLine = string.Empty;
                        }
                    }
                    else
                    {
                        currentLine += c;
                    }
                }
            }

            public void Dispose()
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
            }
        }
    }
}