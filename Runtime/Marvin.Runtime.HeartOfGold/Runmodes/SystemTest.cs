using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Marvin.Configuration;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.HeartOfGold
{
    /// <summary>
    /// Provides a runmode which is used for system tests.
    /// </summary>
    [Runmode(RunmodeName)]
    public class SystemTest : CommandRunmode
    {
        /// <summary>
        /// Name of the runmode.
        /// </summary>
        public const string RunmodeName = "SystemTest";

        private const int BufferSize = 256;

        /// <summary>
        /// Configuration manager instance. Injected by castle.
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        private Timer _shutdownTimer;
        private TcpListener _listener;
        private readonly UTF8Encoding _decoder = new UTF8Encoding();
        private NetworkStream _stream;

        /// <summary>
        /// Boot application (Starts all modules)
        /// </summary>
        protected override void Boot()
        {
            // Override conigs port value
            var portIncrement = int.Parse(Arguments["pi"] ?? "0");
            var wcfConfig = ConfigManager.GetConfiguration<WcfConfig>(false);
            wcfConfig.HttpPort += portIncrement;
            wcfConfig.NetTcpPort += portIncrement;

            // Prepare shutdown timer
            int timeout = int.Parse(Arguments["t"] ?? "300");
            _shutdownTimer = new Timer(ShutDownTimer, null, TimeSpan.FromSeconds(timeout), Timeout.InfiniteTimeSpan);

            // Prepare TCP listener
            int port = int.Parse(Arguments["p"] ?? "23");
            _listener = new TcpListener(IPAddress.Loopback, port);
            _listener.Start();

            // Register to service manager event            
            ModuleManager.ModuleChangedState += OnModuleStateChanged;
            base.Boot();
        }

        /// <summary>
        /// Read commands and pass to execute Command method
        /// </summary>
        protected override void RunTextEnvironment()
        {
            var client = _listener.AcceptTcpClient();
            _stream = client.GetStream();

            var command = string.Empty;
            var buffer = new byte[BufferSize];
            while (command != "exit")
            {
                var read = _stream.Read(buffer, 0, BufferSize);
                command = _decoder.GetString(buffer, 0, read);

                switch (command)
                {
                    case "exit":
                        break;
                    default: ExecuteCommand(command);
                        break;
                }
            }
        }

        /// <summary>
        /// Write the text which should be printed to the connected client.
        /// </summary>
        /// <param name="lines">Text which should be printed.</param>
        protected override void PrintText(params string[] lines)
        {
            if (_stream == null)
                return;

            foreach (var line in lines)
            {
                var bytes = _decoder.GetBytes(line);
                _stream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Print the state change of a module in the console.
        /// </summary>
        /// <param name="sender">Sender object, here IServerModule.</param>
        /// <param name="eventArgs">EventArgs with the new state.</param>
        protected void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs eventArgs)
        {
            lock (this)
            {
                var module = (IServerModule)sender;
                Console.WriteLine("{0} changed state to {1}", module.Name, eventArgs.NewState);
            }
        }

        private void ShutDownTimer(object state)
        {
            try
            {
                ShutDown();
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Shut down application, stop modules, release and dispose the necessary objects.
        /// </summary>
        protected override void ShutDown()
        {
            base.ShutDown();

            ModuleManager.ModuleChangedState -= OnModuleStateChanged;

            _shutdownTimer.Dispose();

            if (_stream != null)
            {
                _stream.Dispose();
            }

            _listener.Stop();

            Thread.Sleep(100);
        }
    }
}
