// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.Runtime.Maintenance.Plugins;
using Moryx.Runtime.Maintenance.Plugins.Logging;

namespace Moryx.TestTools.SystemTest.Clients
{
    public class LogMaintenanceWebClient : TestWebClientBase
    {
        public LogMaintenanceWebClient(int port) : base($"http://localhost:{port}/loggers/")
        {
        }

        public LoggerModel[] GetAllLoggers()
        {
            return Get<LoggerModel[]>("");
        }

        public Task<LoggerModel[]> GetAllLoggersAsync()
        {
            return GetAsync<LoggerModel[]>("");
        }

        public InvocationResponse SetLogLevel(string loggerName, SetLogLevelRequest setLogLevelRequest)
        {
            return PutAsJson<InvocationResponse>($"logger/{loggerName}/loglevel", setLogLevelRequest);
        }

        public Task<InvocationResponse> SetLogLevelAsync(string loggerName, SetLogLevelRequest setLogLevelRequest)
        {
            return PutAsJsonAsync<InvocationResponse>($"logger/{loggerName}/loglevel", setLogLevelRequest);
        }

        public AddAppenderResponse AddAppender(AddAppenderRequest request)
        {
            return PostAsJson<AddAppenderResponse>("appender", request);
        }

        public Task<AddAppenderResponse> AddAppenderAsync(AddAppenderRequest request)
        {
            return PostAsJsonAsync<AddAppenderResponse>("appender", request);
        }

        public InvocationResponse RemoveAppender(string appenderId)
        {
            return DeleteAsJson<InvocationResponse>($"appender/{appenderId}", null);
        }

        public Task<InvocationResponse> RemoveAppenderAsync(string appenderId)
        {
            return DeleteAsJsonAsync<InvocationResponse>($"appender/{appenderId}", null);
        }

        public LogMessageModel[] GetMessages(string appenderId)
        {
            return Get<LogMessageModel[]>($"appender/{appenderId}");
        }

        public Task<LogMessageModel[]> GetMessagesAsync(string appenderId)
        {
            return GetAsync<LogMessageModel[]>($"appender/{appenderId}");
        }
    }
}
