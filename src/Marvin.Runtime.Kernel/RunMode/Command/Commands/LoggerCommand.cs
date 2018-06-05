using System;
using System.Collections.Generic;
using Marvin.Container;
using Marvin.Logging;

namespace Marvin.Runtime.Kernel
{
    internal class ConsoleListener
    {
        public void Log(ILogMessage message)
        {
            // Clear line
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (var i = 0; i < Console.WindowWidth; i++) Console.Write(" ");
            Console.SetCursorPosition(0, currentLineCursor);

            Console.WriteLine("{0} - {1} - {2}: {3}", message.Timestamp, message.Logger.Name, message.Level, message.Message);
        }
    }

    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class LoggerCommand : ICommandHandler
    {
        public IServerLoggerManagement LoggerManagement { get; set; }
        private readonly Dictionary<string, ConsoleListener> _listeners = new Dictionary<string, ConsoleListener>(); 

        private const string CommandName = "logger";

        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == CommandName;
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            if (fullCommand.Length < 2)
            {
                Console.WriteLine("Insufficient number of arguments. Type `help` for more information.");
                return;
            }

            switch (fullCommand[1])
            {
                case "-list":
                    PrintLoggers(LoggerManagement);
                    break;
                case "-add":
                    if (_listeners.ContainsKey(fullCommand[2]))
                        break;
                    var listener = new ConsoleListener();
                    LoggerManagement.AppendListenerToStream(listener.Log, fullCommand[2]);
                    _listeners[fullCommand[2]] = listener;
                    break;
                case "-rem":
                    if (_listeners.ContainsKey(fullCommand[2]))
                        break;
                    LoggerManagement.RemoveListenerFromStream(_listeners[fullCommand[2]].Log);
                    _listeners.Remove(fullCommand[2]);
                    break;
                default: Console.WriteLine("Invalid argument {0}", fullCommand[1]);
                    break;
            }
        }

        private void PrintLoggers(IEnumerable<IModuleLogger> loggers, int tab = 0)
        {
            foreach (var logger in loggers)
            {
                Console.WriteLine("".PadRight(tab) + logger.Name);
                PrintLoggers(logger, tab + 3);
            }
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine(CommandName + " -list".PadRight(pad) + "Print the logger tree");
            Console.WriteLine("       -add <logger>".PadRight(pad) + "Add a log listener for this logger");
            Console.WriteLine("       -rem <logger>".PadRight(pad) + "Removes a log listener for this logger");
        }
    }
}
