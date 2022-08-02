// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.Serialization;

namespace Moryx.Communication.Serial
{
    /// <summary>
    /// Config for binary connections implemented with serial ports
    /// </summary>
    public class SerialBinaryConfig : BinaryConnectionConfig
    {
        /// <inheritdoc />
        public override string PluginName => nameof(SerialBinaryConnection);

        /// <summary>
        /// Port to use for communication
        /// </summary>
        [DataMember]
        public string Port { get; set; }

        /// <summary>
        /// BaudRate to use for communication
        /// </summary>
        [DataMember]
        [DefaultValue(9600)]
        public int BaudRate { get; set; }

        /// <summary>
        /// Parity to use for communication
        /// </summary>
        [DataMember]
        [DefaultValue(Parity.None)]
        public Parity Parity { get; set; }

        /// <summary>
        /// DataBits to use for communication
        /// </summary>
        [DataMember]
        [DefaultValue(DataBitTypes.Eight)]
        public DataBitTypes DataBits { get; set; }

        /// <summary>
        /// Specifies the number of stop bits used on the <see cref="T:System.IO.Ports.SerialPort"></see> object.
        /// </summary>
        [DataMember]
        [DefaultValue(StopBits.One)]
        public StopBits StopBits { get; set; }

        /// <summary>
        /// Handshake to use for communication
        /// </summary>
        [DataMember]
        [DefaultValue(Handshake.None)]
        public Handshake Handshake { get; set; }

        /// <summary>
        /// ReadTimeout to use for communication
        /// </summary>
        [DataMember]
        [DefaultValue(30000)]
        public int ReadTimeout { get; set; }

        /// <summary>
        /// WriteTimeout to use for communication
        /// </summary>
        [DataMember]
        [DefaultValue(20000)]
        public int WriteTimeout { get; set; }

        /// <summary>
        /// ReadBufferSize to use for communication
        /// </summary>
        [DataMember]
        [DefaultValue(100)]
        public int ReadBufferSize { get; set; }

        /// <summary>
        /// WriteBufferSize to use for communication
        /// </summary>
        [DataMember]
        [DefaultValue(100)]
        public int WriteBufferSize { get; set; }
    }
}
