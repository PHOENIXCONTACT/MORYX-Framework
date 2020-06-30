using System;
using System.IO.Ports;
using System.Text;
using Moryx.Logging;

namespace Moryx.Communication.Serial
{
    internal static class SerialPortFactory
    {
        internal static SerialPort FromConfig(SerialBinaryConfig config, IModuleLogger logger)
        {
            logger.Log(LogLevel.Debug, "ConfigComPort started");

            // Create a new _serialPort object with default settings.
            var serialPort = new SerialPort(config.Port);

            try
            {
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

                logger.Log(LogLevel.Debug, "ConfigPort: Opening");

                serialPort.Open();

                logger.Log(LogLevel.Debug, "ConfigPort: Opened");

                return serialPort;
            }
            catch (Exception e)
            {
                logger.LogException(LogLevel.Error, e, "Caught exception while trying to configure port '{0}'", config.Port);

                var msg = new StringBuilder("Known devices:");

                foreach (string s in SerialPort.GetPortNames())
                {
                    msg.AppendLine().AppendFormat("    Device {0}", s);
                }

                logger.Log(LogLevel.Error, msg.ToString());

                throw;
            }
        }
    }
}