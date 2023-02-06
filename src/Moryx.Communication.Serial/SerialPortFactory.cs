// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO.Ports;
using System.Text;
using Microsoft.Extensions.Logging;
using Moryx.Logging;

namespace Moryx.Communication.Serial
{
    internal static class SerialPortFactory
    {
        internal static SerialPort FromConfig(SerialBinaryConfig config, IModuleLogger logger)
        {
            logger.Log(LogLevel.Debug, "ConfigComPort started");

            try
            {
                // Create a new _serialPort object with default settings.
                var serialPort = new SerialPort(config.Port);

                // Allow the user to set the appropriate properties.
                serialPort.BaudRate = config.BaudRate;
                serialPort.Parity = config.Parity;
                serialPort.DataBits = (int)config.DataBits;
                serialPort.StopBits = config.StopBits;
                serialPort.Handshake = config.Handshake;

                // Set the read/write timeouts
                serialPort.ReadTimeout = config.ReadTimeout;
                serialPort.WriteTimeout = config.WriteTimeout;

                serialPort.ReadBufferSize = config.ReadBufferSize;
                serialPort.WriteBufferSize = config.WriteBufferSize;

                return serialPort;
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e, "Caught exception while trying to configure port '{0}'", config.Port);

                var msg = new StringBuilder("Known devices:");

                foreach (var s in SerialPort.GetPortNames())
                {
                    msg.AppendLine().AppendFormat("    Device {0}", s);
                }

                logger.Log(LogLevel.Error, msg.ToString());

                throw;
            }
        }
    }
}