using System;
using System.ComponentModel;
using Marvin.Logging;
using Marvin.Runtime;
using Marvin.Runtime.Modules;
using Marvin.Serialization;

namespace Marvin.DependentTestModule
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        #region Dependency Injection

        public IModuleLogger Logger { get; set; }

        #endregion

        public string ExportDescription(DescriptionExportFormat format)
        {
            switch (format)
            {
                case DescriptionExportFormat.Console: 
                    return ExportConsoleDescription();

                case DescriptionExportFormat.Documentation: 
                    return ExportHtmlDescription();
            }

            return string.Empty;
        }

        // Export your desription for the developer console here
        // This should represent the current state
        private string ExportConsoleDescription()
        {
            var manPage =
@"
Test module for System tests
";
            return manPage;
        }

        // Export your desription for the supervisor or maintenance
        // This should be a static explaination of the plugin
        private string ExportHtmlDescription()
        {
            var manPage =
@"
Test module for System tests
";
            return manPage;
        }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            outputStream("The DependentTestModule does not provide any commands!");
        }

        [EditorVisible, Description("Creates a log message for the given log level")]
        public void CreateTestLogMessage(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    Logger.Log(LogLevel.Trace, "Without a trace...");
                    break;
                case LogLevel.Debug:
                    Logger.Log(LogLevel.Debug, "Debug message");
                    break;
                case LogLevel.Info:
                    Logger.Log(LogLevel.Info, "Infomercial");
                    break;
                case LogLevel.Warning:
                    Logger.Log(LogLevel.Warning, "Test warning.");
                    break;
                case LogLevel.Error:
                    Logger.LogException(LogLevel.Error, new InvalidOperationException("Test exception. Stay cool nothing happened.", new NotImplementedException("That's not true.")), "Message of a test exception.");
                    break;
                case LogLevel.Fatal:
                    Logger.LogException(LogLevel.Fatal, new AccessViolationException("The person you have called is temp..."), "Finish him -> Fatality");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}
