using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.Serialization;

namespace Marvin.Communication.Serial
{
    /// <summary>
    /// Config for binary connections implemented with serial ports
    /// </summary>
    public class SerialBinaryConfig : BinaryConnectionConfig
    {
        ///
        public override string PluginName => SerialBinaryConnection.ConnectionName;

        /// <summary>
        /// Port to use for communication
        /// </summary>
        [DataMember]
        //[AvailableComPorts]
        public string Port { get; set; }

        /// <summary>
        /// Baudrate to use for communication
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
        /// Baudrate to use for communication
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

    /// <summary>
    /// Types of data transmission
    /// </summary>
    public enum DataBitTypes
    {
        /// <summary>
        /// Seven bits at a time
        /// </summary>
        Seven = 7,
        /// <summary>
        /// 8 bits at a time
        /// </summary>
        Eight = 8
    }
}