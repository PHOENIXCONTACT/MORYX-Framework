using Moryx.Container;
using Moryx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Moryx.Asp.Extensions
{
    internal class LoggerExtension : IModuleLogger
    {
        public string Name => "Name";

        public LogLevel ActiveLevel => LogLevel.Debug;

        public IModuleLogger Parent { get => null; }
        IModuleLogger IContainerChild<IModuleLogger>.Parent { get; set; }

        public IModuleLogger Clone(Type targetType)
        {
            throw new NotImplementedException();
        }

        public IModuleLogger GetChild(string name, Type target)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IModuleLogger> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Log(LogLevel level, string message, params object[] formatParameters)
        {
            Console.WriteLine(message, formatParameters);
        }

        public void LogException(LogLevel level, System.Exception ex, string message, params object[] formatParameters)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"{message} Error: {ex.Message}", formatParameters);
            Console.ResetColor();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
