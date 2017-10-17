using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Marvin.Logging;

namespace Marvin.Tools
{
    /// <summary>
    /// A simple TELNET client implementation
    /// </summary>
    public class TelnetClient
    {
        private IPAddress _address;
        private int _tcpPort;
        private StreamWriter _writer;

        /// <summary>Injected property</summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// The TcpClient used for the connection.
        /// </summary>
        public TcpClient TcpClient { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TelnetClient()
        {
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="address">The IP address to connect to</param>
        /// <param name="tcpPort">The TCP port to connect to</param>
        public TelnetClient(IPAddress address, int tcpPort)
        {
            _address = address;
            _tcpPort = tcpPort;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="address">The IP address to connect to</param>
        /// <param name="tcpPort">The TCP port to connect to</param>
        public void Initialize(IPAddress address, int tcpPort)
        {
            _address = address;
            _tcpPort = tcpPort;
        }

        /// <summary>
        /// Tries to connect to the given IP address and TCP port
        /// </summary>
        /// <returns><c>True</c> if the connection could be established or <c>false</c> otherwise.</returns>
        public bool Connect()
        {
            Logger.LogEntry(LogLevel.Debug, "Connecting to {0}:{1}", _address, _tcpPort);

            TcpClient = new TcpClient();

            try
            {
                TcpClient.Connect(_address, _tcpPort);
                _writer = new StreamWriter(TcpClient.GetStream());

            }
            catch (Exception e)
            {
                Logger.LogException(LogLevel.Error, e, "Cant connect to  {0}:{1}", _address, _tcpPort);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes an exisiting connection.
        /// </summary>
        public void Close()
        {
            if (_writer != null)
            {
                _writer.Close();
            }

            if (TcpClient != null)
            {
                TcpClient.Close();
                TcpClient = null;
            }
        }

        /// <summary>
        /// Sends a line of text to the server. If the connection is not yet open, the method tries to open it.
        /// </summary>
        /// <param name="line">The text to send.</param>
        /// <returns><c>True</c> if the connection is opened and the text could be send. <c>False</c> if the connection could not be opened.</returns>
        public bool WriteLine(string line)
        {
            if (TcpClient == null)
            {
                if (! Connect())
                {
                    return false;
                }
            }

            _writer.WriteLine(line);
            _writer.Flush();

            return true;
        }
    }
}