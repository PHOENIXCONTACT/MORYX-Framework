namespace Marvin.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    public class EndDelimiterOnlyInterpreter : DelimitedMessageInterpreter
    {
        public const int TestBufferSize = 1024;
        /// <summary>
        /// Size of the read buffer
        /// </summary>
        protected override int BufferSize
        {
            get { return TestBufferSize; }
        }

        public const int TestReadSize = 100;
        /// <summary>
        /// Number of bytes to read in each iteration
        /// </summary>
        protected override int ReadSize
        {
            get { return TestReadSize; }
        }

        public static byte[] TestEndDelimiter = { 1, 3, 3, 7 };
        /// <summary>
        /// Byte sequence for end of message
        /// </summary>
        protected override byte[] EndDelimiter
        {
            get { return TestEndDelimiter; }
        } 
    }
}