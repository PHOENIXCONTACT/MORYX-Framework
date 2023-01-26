// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.DependentTestModule
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        #region Dependency Injection

        public IModuleLogger Logger { get; set; }

        #endregion

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            outputStream("The DependentTestModule does not provide any commands!");
        }

        [EntrySerialize, Description("Creates a log message for the given log level")]
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
                case LogLevel.Information:
                    Logger.Log(LogLevel.Information, "Infomercial");
                    break;
                case LogLevel.Warning:
                    Logger.Log(LogLevel.Warning, "Test warning.");
                    break;
                case LogLevel.Error:
                    Logger.Log(LogLevel.Error, new InvalidOperationException("Test exception. Stay cool nothing happened.", new NotImplementedException("That's not true.")), "Message of a test exception.");
                    break;
                case LogLevel.Critical:
                    Logger.Log(LogLevel.Critical, new AccessViolationException("The person you have called is temp..."), "Finish him -> Fatality");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}
