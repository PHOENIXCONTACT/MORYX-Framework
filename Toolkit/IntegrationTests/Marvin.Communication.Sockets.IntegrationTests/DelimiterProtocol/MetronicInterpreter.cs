using System.Text;

namespace Marvin.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    public class MetronicInterpreter : DelimitedMessageInterpreter
    {
        private static MetronicInterpreter _instance;
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static MetronicInterpreter Instance
        {
            get { return _instance ?? (_instance = new MetronicInterpreter()); }
        }

        /// <summary>
        /// 1 MB read buffer size for each connection
        /// </summary>
        protected override int BufferSize
        {
            get { return 1048576; }
        }

        /// <summary>
        /// Number of bytes to read in each iteration
        /// </summary>
        protected override int ReadSize
        {
            get { return 10240; }
        }

        /// <summary>
        /// Byte sequence for start of message
        /// </summary>
        protected override byte[] StartDelimiter
        {
            get { return Encoding.UTF8.GetBytes("<GP>"); }
        }

        /// <summary>
        /// Byte sequence for end of message
        /// </summary>
        protected override byte[] EndDelimiter
        {
            get { return Encoding.UTF8.GetBytes("</GP>"); }
        }
    }
}