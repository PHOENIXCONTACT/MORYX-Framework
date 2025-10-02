// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics.Logger;

namespace Moryx.AspNetCore.Mqtt.Components;
internal class MqttClientLogger(ILogger<MqttClientLogger> logger) : IMqttNetLogger
{
    public bool IsEnabled => true;

    public void Publish(MqttNetLogLevel logLevel, string source, string message, object[] parameters, Exception exception)
    {
        var logMessage = string.Format("{0} - {1}", source, message);
        switch (logLevel)
        {
            case MqttNetLogLevel.Verbose:
                logger.Log(LogLevel.Debug, logMessage, parameters, exception);
                break;
            case MqttNetLogLevel.Info:
                logger.Log(LogLevel.Information, logMessage, parameters, exception);
                break;
            case MqttNetLogLevel.Warning:
                logger.Log(LogLevel.Warning, logMessage, parameters, exception);
                break;
            case MqttNetLogLevel.Error:
                logger.Log(LogLevel.Error, logMessage, parameters, exception);
                break;
        }
    }
}
