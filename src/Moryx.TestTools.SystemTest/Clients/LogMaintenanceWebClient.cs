// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Moryx.Runtime.Maintenance.Contracts;
using Moryx.Runtime.Maintenance.Logging;

namespace Moryx.TestTools.SystemTest.Clients
{
    public class LogMaintenanceWebClient : TestWebClientBase
    {
        public LogMaintenanceWebClient(int port) : base($"http://localhost:{port}/LogMaintenance/")
        {
        }

        public LoggerModel[] GetAllLoggers()
        {
            return Get<LoggerModel[]>("loggers");
        }

        public async Task<LoggerModel[]> GetAllLoggersAsync()
        {
            return await GetAsync<LoggerModel[]>("loggers");
        }

        public InvocationResponse SetLogLevel(string loggerName, SetLogLevelRequest setLogLevelRequest)
        {
            return PostAsJson<InvocationResponse>($"loggers/{loggerName}/loglevel", setLogLevelRequest);
        }

        public async Task<InvocationResponse> SetLogLevelAsync(string loggerName, SetLogLevelRequest setLogLevelRequest)
        {
            return await PostAsJsonAsync<InvocationResponse>($"loggers/{loggerName}/loglevel", setLogLevelRequest);
        }

        public AddAppenderResponse AddAppender(AddAppenderRequest request)
        {
            return PutAsJson<AddAppenderResponse>("appender", request);
        }

        public async Task<AddAppenderResponse> AddAppenderAsync(AddAppenderRequest request)
        {
            return await PutAsJsonAsync<AddAppenderResponse>("appender", request);
        }

        public InvocationResponse RemoveAppender(string appenderId)
        {
            return DeleteAsJson<InvocationResponse>($"appender/{appenderId}", null);
        }

        public async Task<InvocationResponse> RemoveAppenderAsync(string appenderId)
        {
            return await DeleteAsJsonAsync<InvocationResponse>($"appender/{appenderId}", null);
        }

        public LogMessageModel[] GetMessages(string appenderId)
        {
            return Get<LogMessageModel[]>($"appender/{appenderId}");
        }

        public async Task<LogMessageModel[]> GetMessagesAsync(string appenderId)
        {
            return await GetAsync<LogMessageModel[]>($"appender/{appenderId}");
        }
    }
}
