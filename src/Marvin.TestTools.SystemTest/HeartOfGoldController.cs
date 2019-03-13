using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using Marvin.Model;
using Marvin.Runtime.Modules;
using Marvin.TestTools.SystemTest.Clients;
using Marvin.TestTools.SystemTest.DatabaseMaintenance;
using Marvin.TestTools.SystemTest.Logging;
using Marvin.TestTools.SystemTest.Maintenance;
using InvocationResponse = Marvin.TestTools.SystemTest.DatabaseMaintenance.InvocationResponse;
using LogLevel = Marvin.Logging.LogLevel;

namespace Marvin.TestTools.SystemTest
{
    /// <summary>
    /// Exception in case of a not found service
    /// </summary>
    public class MarvinServiceNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarvinServiceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MarvinServiceNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarvinServiceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public MarvinServiceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Helper to remote control the heart of gold
    /// </summary>
    public class HeartOfGoldController : IDisposable
    {
        /// <summary>
        /// Nested class to combine the id of the logger and the log message itself.
        /// </summary>
        public class LoggerEventArgs
        {
            /// <summary>
            /// Id of a registered logger.
            /// </summary>
            public int LoggerId { get; set; }
            /// <summary>
            /// The current log messages which occured.
            /// </summary>
            public LogMessageModel[] Messages { get; set; }
        }

        /// <summary>
        /// Publish log message when they are received.
        /// </summary>
        public event EventHandler<LoggerEventArgs> LogMessagesReceived;

        /// <summary>
        /// Default telnet port - will be increased by the port increment
        /// </summary>
        private const int DefaultTelnetPort = 23;

        /// <summary>
        /// Default http port - will be increased by the port increment
        /// </summary>
        private const int DefaultHttpPort = 80;

        /// <summary>
        /// Default net.tcp port - will be increased by the port increment
        /// </summary>
        private const int DefaultNetTcpPort = 816;

        /// <summary>
        /// Currently defined port increment
        /// </summary>
        private readonly int _portIncrement;

        /// <summary>
        /// Timer to get the log-entrys
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// WCF service to the HoG to configure and control services
        /// </summary>
        private ModuleMaintenanceWebClient _maintenanceClient;
        /// <summary>
        /// WCF service to the HoG to get append to loggers and get log-entrys
        /// </summary>
        private LogMaintenanceWebClient _loggingClient;
        /// <summary>
        /// WCF service to the HoG to change and check the database configurations.
        /// </summary>
        private DatabaseMaintenanceWebClient _databaseClient;

        /// <summary>
        /// List of all append logger ids
        /// </summary>
        private readonly ICollection<int> _loggerIds = new List<int>();

        /// <summary>
        /// Starts and stops the Heart of Gold Executable
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// Name of the executable under test
        /// </summary>
        public string ApplicationExeName { get; set; } = "HeartOfGold.exe";

        /// <summary>
        /// Gets or sets the directory of the runtime executable.
        /// </summary>
        public string RuntimeDir { get; set; }

        /// <summary>
        /// Gets or sets the directory of the config files.
        /// </summary>
        public string ConfigDir { get; set; }

        /// <summary>
        /// Get or set the Telnet port number
        /// </summary>
        public int TelnetPort { get; set; }
        
        /// <summary>
        /// Currently used http port
        /// </summary>
        public int HttpPort { get; set; }

        /// <summary>
        /// Currently used net.tcp port
        /// </summary>
        public int NetTcpPort { get; set; }

        /// <summary>
        /// Gets or sets the directory of the config files.
        /// </summary>
        public int ExecutionTimeout { get; set; }

        /// <summary>
        /// Interval in which the log messages are read.
        /// </summary>
        public int TimerInterval { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeartOfGoldController" /> class.
        /// </summary>
        public HeartOfGoldController()
        {
            // Check for port increment
            var portIncrement = Environment.GetEnvironmentVariable("PORT_INCREMENT");
            if (portIncrement != null)
            {
                int portIncrementNum;

                if (int.TryParse(portIncrement, out portIncrementNum))
                {
                    // Add Jenkins build processor number to port number to allow parallel execution of system tests.
                    _portIncrement = portIncrementNum;
                }
            }

            TelnetPort = DefaultTelnetPort + _portIncrement;
            HttpPort = DefaultHttpPort + _portIncrement;
            NetTcpPort = DefaultNetTcpPort + _portIncrement;

            ExecutionTimeout = 0;
            TimerInterval = 1000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeartOfGoldController" /> class.
        /// </summary>
        /// <param name="buildDirectoryName">The build directory. Default is 'Build'</param>
        /// <param name="runtimeDirectoryName">The runtime directory. Default is 'ServiceRuntime'</param>
        /// <param name="configDirecoryName">The configuration direcory. Default is 'Config'</param>
        public HeartOfGoldController(string buildDirectoryName, string runtimeDirectoryName, string configDirecoryName) : this()
        {
            // Search for the runtime executable with the given directorys
            SearchForRuntime(buildDirectoryName, runtimeDirectoryName, configDirecoryName);
        }

        /// <summary>
        /// Starts the HeartOfGold executable.
        /// </summary>
        /// <returns><c>true</c> if start succeeds, <c>false</c> if not</returns>
        public bool StartHeartOfGold()
        {
            return StartHeartOfGold(ApplicationExeName);
        }

        /// <summary>
        /// Starts the HeartOfGold.
        /// </summary>
        /// <param name="exeName">Name of the executable.</param>
        /// <returns><c>true</c> if start succeeds, <c>false</c> if not</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Can't start HeartOfGold without RuntimeDir.
        /// or
        /// Can't start HeartOfGold without ConfigDir.
        /// or
        /// HeartOfGold is already running.
        /// </exception>
        private bool StartHeartOfGold(string exeName)
        {
            // check the runtime directory
            if (RuntimeDir == null)
                throw new InvalidOperationException("Can't start HeartOfGold without RuntimeDir.");

            // check the config directory
            if (ConfigDir == null)
                throw new InvalidOperationException("Can't start HeartOfGold without ConfigDir.");

            // check that the process is not allready running 
            if (Process != null && !Process.HasExited)
                throw new InvalidOperationException("HeartOfGold is already running.");

            var wcfConfig = Path.Combine(RuntimeDir, ConfigDir, "Marvin.Tools.Wcf.WcfConfig.mcf");
            if (File.Exists(wcfConfig))
                File.Delete(wcfConfig);

            var runtimeCommand = Path.Combine(RuntimeDir, exeName);

            Process = new Process
            {
                // Start the heart of gold in developer mode to start as a console application (-d) and set the path to the config files (-c=path) 
                StartInfo = new ProcessStartInfo(runtimeCommand,
                    $"-d -r=SystemTest -c={ConfigDir} -p={TelnetPort} -t={ExecutionTimeout} -pi={_portIncrement}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            DataReceivedEventHandler outputDelegate = (sender, args) => 
                Console.WriteLine("Runtime > " + args.Data);

            Process.OutputDataReceived += outputDelegate;
            Process.ErrorDataReceived += outputDelegate;

            try
            {
                // start now
                Process.Start();

                // begin to read output
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception while trying to start command '{0}': {1}", runtimeCommand, e);
                return false;
            }
        }

        /// <summary>
        /// Create the WCF and telnet clients needed to communicate with the server under test.
        /// </summary>
        public void CreateClients()
        {
            // get the wcf services of the started instance
            _loggingClient = new LogMaintenanceWebClient(HttpPort);
            _maintenanceClient = new ModuleMaintenanceWebClient(HttpPort);
            _databaseClient = new DatabaseMaintenanceWebClient(HttpPort);
        }

        /// <summary>
        /// Creates a binding for WebHttp
        /// </summary>
        /// <returns></returns>
        public static Binding CreateBasicWebBinding()
        {
            var webHttpBinding = new WebHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                CloseTimeout = TimeSpan.FromSeconds(30),
                OpenTimeout = TimeSpan.FromSeconds(30),
                ReceiveTimeout = TimeSpan.FromSeconds(30),
                SendTimeout = TimeSpan.FromSeconds(30),
                UseDefaultWebProxy = true,
                ReaderQuotas =
                {
                    MaxStringContentLength = int.MaxValue,
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue
                },
                Security =
                {
                    Mode = WebHttpSecurityMode.None
                }
            };

            return webHttpBinding;
        }

        /// <summary>
        /// Creates a binding for BasicHttp
        /// </summary>
        /// <returns></returns>
        public static Binding CreateBasicHttpBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                ReceiveTimeout = new TimeSpan(0, 0, 0, 30),
                SendTimeout = new TimeSpan(0, 0, 0, 30),
                OpenTimeout = new TimeSpan(0, 0, 0, 30),
                CloseTimeout = new TimeSpan(0, 0, 0, 30),
                ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue,
                    MaxStringContentLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue
                },
            };

            return binding;
        }


        /// <summary>
        /// Stops the started HeartOfGold instance.
        /// </summary>
        /// <param name="timeoutInSeconds">The number of seconds to wait for the process to finish.</param>
        /// <returns>True if the HeartOfGold instance was stopped successfully, false otherwise</returns>
        public bool StopHeartOfGold(int timeoutInSeconds)
        {
            RemoveRemoteLogAppender();

            var success = true;
            var tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(new IPEndPoint(IPAddress.Loopback, TelnetPort));
                var exitBytes = new UTF8Encoding().GetBytes("exit");
                using (var stream = tcpClient.GetStream())
                {
                    stream.Write(exitBytes, 0, exitBytes.Length);
                }
            }
            catch
            {
                success = false;
            }
            finally
            {
                tcpClient.Close();
                Process.WaitForExit(timeoutInSeconds * 1000);
            }

            return success;
        }

        /// <summary>
        /// Kills the started HeartOfGold instance.
        /// </summary>
        public void KillHeartOfGold()
        {
            RemoveRemoteLogAppender();

            Process.Kill();
        }

        /// <summary>
        /// Stops the given service on the heart of gold instance.
        /// </summary>
        /// <param name="service">The service to stop</param>
        public void StopService(string service)
        {
            WaitForServiceCall(() => _maintenanceClient.Stop(service));
        }

        /// <summary>
        /// Stops the given service on the heart of gold instance.
        /// </summary>
        /// <param name="service">The service to stop</param>
        public void StopServiceAsync(string service)
        {
            WaitForServiceCall(() => _maintenanceClient.StopAsync(service));
        }

        /// <summary>
        /// Stops the given service on the heart of gold instance.
        /// </summary>
        /// <param name="service">The service to stop</param>
        public void StartService(string service)
        {
            WaitForServiceCall(() => _maintenanceClient.Start(service));
        }

        /// <summary>
        /// Stops the given service on the heart of gold instance.
        /// </summary>
        /// <param name="service">The service to stop</param>
        public void StartServiceAsync(string service)
        {
            WaitForServiceCall(() => _maintenanceClient.StartAsync(service));
        }

        /// <summary>
        /// Reincarnates (Restarts) the given service on the heart of gold instance.
        /// </summary>
        /// <param name="service">The service to reincarnate.</param>
        public void ReincarnateService(string service)
        {
            WaitForServiceCall(() => _maintenanceClient.Reincarnate(service));
        }

        /// <summary>
        /// Reincarnates (Restarts) the given service on the heart of gold instance.
        /// </summary>
        /// <param name="service">The service to reincarnate.</param>
        public void ReincarnateServiceAsync(string service)
        {
            WaitForServiceCall(() => _maintenanceClient.ReincarnateAsync(service));
        }

        /// <summary>
        /// Waits for a given service until it reaches the given state.
        /// </summary>
        /// <param name="service">The service to wait for</param>
        /// <returns>True if the service reached the Running state, false otherwise</returns>
        public bool WaitForService(string service)
        {
            string[] services = { service };

            return WaitForServices(services, ServerModuleState.Running, 30.0);
        }

        /// <summary>
        /// Waits for a given service until it reaches the given state.
        /// </summary>
        /// <param name="service">The service to wait for</param>
        /// <param name="state">The state that should be reached by the service.</param>
        /// <returns>True if the service reached the expected state, false otherwise</returns>
        public bool WaitForService(string service, ServerModuleState state)
        {
            string[] services = { service };

            return WaitForServices(services, state, 30.0);
        }

        /// <summary>
        /// Waits for a given service until it reaches the given state.
        /// </summary>
        /// <param name="service">The service to wait for</param>
        /// <param name="state">The state that should be reached by the service.</param>
        /// <param name="timeoutInSeconds">The timeout for the service to reach the given state; in seconds.</param>
        /// <returns>True if the service reached the expected state, false otherwise</returns>
        public bool WaitForService(string service, ServerModuleState state, double timeoutInSeconds)
        {
            string[] services = { service };

            return WaitForServices(services, state, timeoutInSeconds);
        }

        /// <summary>
        /// Waits for a number of given services until they reach the Running state.
        /// </summary>
        /// <param name="moduleNames">The services to wait for</param>
        /// <returns>True if all services reached the Running state, false otherwise</returns>
        public bool WaitForServices(string[] moduleNames)
        {
            return WaitForServices(moduleNames, ServerModuleState.Running, 30.0);
        }

        /// <summary>
        /// Waits for a number of given services until they reach the given state.
        /// </summary>
        /// <param name="moduleNames">The services to wait for</param>
        /// <param name="state">The state that should be reached by the services.</param>
        /// <returns>True if all services reached the expected state, false otherwise</returns>
        public bool WaitForServices(string[] moduleNames, ServerModuleState state)
        {
            return WaitForServices(moduleNames, state, 30.0);
        }

        /// <summary>
        /// Waits for a number of given services until they reach the given state.
        /// </summary>
        /// <param name="moduleNames">The services to wait for</param>
        /// <param name="state">The state that should be reached by the services.</param>
        /// <param name="timeoutInSeconds">The timeout for the services to reach the given state; in seconds.</param>
        /// <returns>True if all services reached the expected state, false otherwise</returns>
        public bool WaitForServices(string[] moduleNames, ServerModuleState state, double timeoutInSeconds)
        {
            // Calculate timeout time.
            DateTime timeout = DateTime.Now.AddSeconds(timeoutInSeconds);

            // Check service states until the timeout accures or the given state has bin reached.
            while (DateTime.Now < timeout)
            {
                ServerModuleModel[] moduleModels;

                try
                {
                    // Get all existing services of the heart of gold instance.
                    moduleModels = _maintenanceClient.GetAll();
                }
                catch (FaultException)
                {
                    // This may happen while HeartOfGold is restarting services.

                    Thread.Sleep(100);

                    continue;
                }
                catch (WebException)
                {
                    // This may happen while HeartOfGold is starting

                    Thread.Sleep(100);

                    continue;
                }
                catch (ServerTooBusyException)
                {
                    // This may happen while HeartOfGold is starting

                    Thread.Sleep(100);

                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Caught exception {0}", ex);

                    throw;
                }

                // Will be set to false if any service didn't reached the given state.
                var statesReached = true;

                // Search the plugins with the given names
                foreach (var service in moduleNames)
                {
                    var module = moduleModels.FirstOrDefault(m => m.Name == service);

                    if (module == null)
                        throw new MarvinServiceNotFoundException($"No service found with name '{service}'!");

                    // Check the state of the plugin
                    if (module.HealthState == state)
                        continue;

                    statesReached = false;
                    break;
                }

                // Check if all services have reached the given state
                if (statesReached)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get all Pluginlogger from the HoG.
        /// </summary>
        /// <returns>Array of all PluginLogger which could be fetched.</returns>
        public LoggerModel[] GetAllPluginLogger()
        {
            return WaitForServiceCall(() => _loggingClient.GetAllLoggers());
        }

        /// <summary>
        /// Set the log level of a specific pluginlogger on the HoG.
        /// </summary>
        /// <param name="logger">The logger which should be set.</param>
        /// <param name="level">The level which should be set.</param>
        public void SetLogLevel(LoggerModel logger, LogLevel level)
        {
            WaitForServiceCall(() => _loggingClient.SetLogLevel(logger.Name, new SetLogLevelRequest { Level = level }));
        }

        /// <summary>
        /// Connect to a heart of gold client logger.
        /// Used to receive the logging entrys generated by the heart of gold instance.
        /// </summary>
        /// <param name="logger">The name of the logger to connect to.</param>
        /// <param name="level">The logging level to get.</param>
        /// <returns>The logger identifier.</returns>
        public int AddRemoteLogAppender(string logger, LogLevel level)
        {
            var response = WaitForServiceCall(() => _loggingClient.AddAppender(new AddAppenderRequest { MinLevel = level, Name = logger } ));

            // Add to the list of loggers
            lock (_loggerIds)
            {
                _loggerIds.Add(response.AppenderId);
            }

            lock (this)
            {
                if (_timer == null)
                {
                    // initialize the timer to get the logging entrys
                    _timer = new Timer(ReadLogTimerElapsed, null, TimerInterval, Timeout.Infinite);
                }
            }

            return response.AppenderId;
        }

        /// <summary>
        /// Disconnect from the heart of gold client logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger</param>
        /// <param name="loggerId">The logger identifier.</param>
        public void RemoveRemoteLogAppender(string loggerName, int loggerId)
        {
            WaitForServiceCall(() => _loggingClient.RemoveAppender(loggerId.ToString()));
            lock (_loggerIds)
            {
                _loggerIds.Remove(loggerId);
            }
        }

        /// <summary>
        /// Disconnect from all heart of gold client logger.
        /// </summary>
        public void RemoveRemoteLogAppender()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            foreach (int loggerId in _loggerIds)
            {
                // The compiler generates a warning when using a foreach var in a closure...
                var loggerIdCompileSave = loggerId;
                WaitForServiceCall(() => _loggingClient.RemoveAppender(loggerIdCompileSave.ToString()));
            }

            lock (_loggerIds)
            {
                _loggerIds.Clear();
            }
        }

        /// <summary>
        /// Tries to execute a method until it succeeds or the timeout occures.
        /// Catches EndpointNotFoundExceptions of wcf services and retrys to execute the method after 100ms.
        /// </summary>
        /// <typeparam name="T">Type of the return value of the <para>taskToTry</para></typeparam>
        /// <param name="taskToTry">The task to try.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        /// <returns>return value of the executed task</returns>
        private T WaitForServiceCall<T>(Func<T> taskToTry, double timeoutInSeconds = 5.0)
        {
            // calculate max time for execution
            var timeoutTime = DateTime.Now.AddSeconds(timeoutInSeconds);

            Exception lastException = null;

            while (DateTime.Now < timeoutTime)
            {
                try
                {
                    var result = taskToTry();
                    return result;
                }
                catch (EndpointNotFoundException ex)
                {
                    // remember last exception to throw at last
                    lastException = ex;
                    Console.WriteLine("Endpoint was not found, retry in 100ms...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Caught exception {0}", ex);

                    throw;
                }

                Thread.Sleep(100);
            }

            if (lastException != null)
            {
                throw lastException;
            }

            throw new Exception($"WaitForServiceCall timeed out after {timeoutInSeconds} seconds");
        }

        /// <summary>
        /// Tries to execute a method until it succeeds or the timeout occures.
        /// Catches EndpointNotFoundExceptions of wcf services and retrys to execute the method after 100ms.
        /// </summary>
        /// <param name="taskToTry">The task to try.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds.</param>
        private static void WaitForServiceCall(Action taskToTry, double timeoutInSeconds = 5.0)
        {
            var timeoutTime = DateTime.Now.AddSeconds(timeoutInSeconds);

            Exception lastException = null;

            while (DateTime.Now < timeoutTime)
            {
                try
                {
                    taskToTry();
                    return; // Execution was successfull; return now!
                }
                catch (EndpointNotFoundException ex)
                {
                    // remember last exception to throw at last
                    lastException = ex;
                    Console.WriteLine("Endpoint was not found, retry in 100ms...");
                }
                catch (Exception ex)
                {
                    // remember exeption and throw at last.
                    lastException = ex;
                    break;
                }

                Thread.Sleep(100);
            }

            throw lastException;
        }

        /// <summary>
        /// Disposes this instance of the  <see cref="HeartOfGoldController"/> class.
        /// </summary>
        public void Dispose()
        {
            RemoveRemoteLogAppender();

            if (Process != null)
            {
                Process.Close();
                Process.Dispose();

                Process = null;
            }

            if (_timer != null)
            {
                _timer.Dispose();

                _timer = null;
            }
        }

        /// <summary>
        /// Searches for the runtime directorys.
        /// </summary>
        /// <param name="buildDirectoryName">Name of the build directory.</param>
        /// <param name="runtimeDirectoryName">Name of the runtime directory.</param>
        /// <param name="configDirectoryName">Name of the configuration direcory.</param>
        private void SearchForRuntime(string buildDirectoryName, string runtimeDirectoryName, string configDirectoryName)
        {
            DirectoryInfo buildDir = null;

            // Get current directory
            var assembly = GetType().Assembly;
            string location = assembly.Location;
            var fi = new FileInfo(location);
            var currentDir = fi.Directory;

            // Search for the "build" folder in the current directory.
            while (buildDir == null && currentDir != null)
            {
                // Is the current directory the "build" directory?
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (currentDir.Name == buildDirectoryName)
                    buildDir = currentDir;
                else
                    // Try to find the "build directory in the subfolders. buildDir will stay null when it is not found.
                    buildDir = currentDir.EnumerateDirectories(buildDirectoryName).FirstOrDefault();

                // go one folder backwards with the current directory
                currentDir = currentDir.Parent;
            }

            if (buildDir == null)
                throw new DirectoryNotFoundException($"The build directory '{buildDirectoryName}' could not be found!");

            // Find the runtime directory
            var runtimeDir = buildDir.EnumerateDirectories(runtimeDirectoryName).FirstOrDefault();

            if (runtimeDir == null)
                throw new DirectoryNotFoundException($"The runtime directory '{runtimeDirectoryName}' could not be found!");
            RuntimeDir = runtimeDir.FullName;

            // find the config directory
            var configDir = runtimeDir.EnumerateDirectories(configDirectoryName).FirstOrDefault();

            if (configDir == null)
                throw new DirectoryNotFoundException($"The config directory '{configDirectoryName}' could not be found!");

            ConfigDir = configDir.FullName;
        }

        private void ReadLogTimerElapsed(object state)
        {
            // Read logentrys of all registered loggers
            int[] loggerIds;

            lock (_loggerIds)
            {
                loggerIds = _loggerIds.ToArray();
            }

            foreach (int loggerId in loggerIds)
            {
                try
                {
                    // Get log messages
                    var logResult = WaitForServiceCall(() => _loggingClient.GetMessages(loggerId.ToString()));

                    // Write the logmessages to the console to be readable in the jenkins output.
                    foreach (LogMessageModel message in logResult)
                    {
                        Console.WriteLine("{0:HH:mm:ss} RemoteLog:\nTimestamp: {1} | Level: {2} | Class: {3} | Message: {4}",
                                          DateTime.Now, message.Timestamp, message.LogLevel, message.ClassName, message.Message);
                    }

                    LogMessagesReceived?.Invoke(this, new LoggerEventArgs
                    {
                        LoggerId = loggerId,
                        Messages = logResult
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't get log messages: {0}", e.Message);
                }
            }

            // Restart timer
            _timer?.Change(TimerInterval, Timeout.Infinite);
        }


        /// <summary>
        /// Gets the 'trunk' configuration of a given service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>The 'trunk' configuration</returns>
        public Config GetConfig(string serviceName)
        {
            return WaitForServiceCall(() => _maintenanceClient.GetConfig(serviceName));
        }

        /// <summary>
        /// Sets the configuration of a given service.
        /// The change can be done on the 'trunk'-configuration or in a sub-configuration entry.
        /// </summary>
        public void SetConfig(Config config, string moduleName)
        {
            WaitForServiceCall(() => _maintenanceClient.SetConfig(moduleName, new SaveConfigRequest { Config = config, UpdateMode = ConfigUpdateMode.UpdateLiveAndSave }));
        }

        /// <summary>
        /// Creates an UnitOfWorkFactory.
        /// </summary>
        public static FactoryCreationContext<TFactory> CreateUnitOfWorkFactory<TFactory>(DatabaseConfigModel databaseConfigModel)
            where TFactory : class, IUnitOfWorkFactory, new()
        {
            var creationContext = new FactoryCreationContext<TFactory>();
            creationContext.SetConfig(databaseConfigModel);
            return creationContext;
        }

        /// <summary>
        /// Creates the database configuration.
        /// </summary>
        /// <param name="server">The database server address.</param>
        /// <param name="database">The database name.</param>
        /// <param name="user">The user name.</param>
        /// <param name="password">The user password.</param>
        /// <param name="port">The database server port.</param>
        /// <returns>Created database configuration.</returns>
        public static DatabaseConfigModel CreateDatabaseConfig(string server, string database, string user, string password, int port = 5432)
        {
            return CreateDatabaseConfig(server, database, user, password, "public", port);
        }

        /// <summary>
        /// Creates the database configuration.
        /// </summary>
        /// <param name="server">The database server address.</param>
        /// <param name="database">The database name.</param>
        /// <param name="user">The user name.</param>
        /// <param name="password">The user password.</param>
        /// <param name="schema">The schema name.</param>
        /// <param name="port">The database server port.</param>
        /// <returns>Created database configuration.</returns>
        public static DatabaseConfigModel CreateDatabaseConfig(string server, string database, string user, string password, string schema, int port = 5432)
        {
            // Initalize the database config
            DatabaseConfigModel databaseConfigModel = new DatabaseConfigModel
            {
                Server = server,
                Database = database,
                User = user,
                Password = password,
                Port = port
            };

            return databaseConfigModel;
        }

        /// <summary>
        /// Checks if the database allready exists.
        /// </summary>
        /// <param name="databaseConfigModel">The database configuration to check.</param>
        /// <param name="targetModel">The model name.</param>
        /// <returns><c>true</c> if the database exists, <c>false</c> if it do not exist or the connection was not posible.</returns>
        public TestConnectionResponse CheckDatabase(DatabaseConfigModel databaseConfigModel, string targetModel)
        {
            // Check if database allready exists
            return WaitForServiceCall(() => _databaseClient.TestDatabaseConfig(targetModel, databaseConfigModel));
        }

        /// <summary>
        /// Creates the database as defined in the database configuration.
        /// </summary>
        /// <param name="databaseConfigModel">The database configuration with the create informations.</param>
        /// <param name="targetModel">The model name.</param>
        /// <returns><c>true</c> if the database was created successfully, <c>false</c> if not</returns>
        public bool CreateDatabase(DatabaseConfigModel databaseConfigModel, string targetModel)
        {
            var result = WaitForServiceCall(() => _databaseClient.CreateDatabase(targetModel, databaseConfigModel));

            // result is "" when it was successfull.
            if (result.Success)
                return true;

            Console.WriteLine("Database creation failed with error message: {0}", result.ErrorMessage);
            return false;
        }

        /// <summary>
        /// Deletes the database as defined in the database configuration.
        /// </summary>
        /// <param name="databaseConfigModel">The database configuration with the delete informations.</param>
        /// <param name="targetModel">The model name.</param>
        /// <returns><c>true</c> if the database has been deleted successfully, <c>false</c> if not</returns>
        public bool DeleteDatabase(DatabaseConfigModel databaseConfigModel, string targetModel)
        {
            var result = WaitForServiceCall(() => _databaseClient.EraseDatabase(targetModel, databaseConfigModel));

            // result is "" when it was successfull.
            if (result.Success)
                return true;

            Console.WriteLine("Database deletion failed with error message: {0}", result.ErrorMessage);
            return false;
        }

        /// <summary>
        /// Backups the database and stores the backup to the ".\Backups\" folder.
        /// </summary>
        public InvocationResponse DumpDatabase(DatabaseConfigModel databaseConfigModel, string targetModel)
        {
            var result = WaitForServiceCall(() => _databaseClient.DumpDatabase(targetModel, databaseConfigModel));

            // result is "" when it was successfull.
            if (result.Success)
                return result;

            Console.WriteLine("Database dump failed with error message: {0}", result.ErrorMessage);
            return null;
        }

        /// <summary>
        /// Restores the database.
        /// </summary>
        public bool RestoreDatabase(DatabaseConfigModel databaseConfigModel, string targetModel, string databaseFile)
        {
            var result = WaitForServiceCall(() => _databaseClient.RestoreDatabase(targetModel, new RestoreDatabaseRequest { BackupFileName = databaseFile, Config = databaseConfigModel }));

            // result is "" when it was successfull.
            if (result.Success)
                return true;

            Console.WriteLine("Database restore failed with error message: {0}", result);
            return false;
        }

        /// <summary>
        /// Executes a database setup
        /// </summary>
        public bool ExecuteSetup(DatabaseConfigModel databaseConfigModel, string targetModel, SetupModel setupModel)
        {
            var result = WaitForServiceCall(() => _databaseClient.ExecuteSetup(targetModel, new ExecuteSetupRequest { Config = databaseConfigModel, Setup = setupModel }));

            if (result.Success)
                return true;

            Console.WriteLine("Setup execution failed with error message: {0}", result);
            return false;
        }

        /// <summary>
        /// Save and activates the database configuration.
        /// The services may restart cause of the changed database configuration!
        /// </summary>
        /// <param name="databaseConfigModel">The database configuration to save and activate.</param>
        /// <param name="targetModel">The model name.</param>
        public void ActivateDatabase(DatabaseConfigModel databaseConfigModel, string targetModel)
        {
            try
            {
                // activate the new database. this will reincanate the services on the heart of gold
                WaitForServiceCall(() => _databaseClient.SetDatabaseConfig(targetModel, databaseConfigModel));
            }
            catch
            {
                // Sometimes there comes an exception because the service has stopped on the HoG
            }
            finally
            {
                Thread.Sleep(1000);
            }
        }
    }
}