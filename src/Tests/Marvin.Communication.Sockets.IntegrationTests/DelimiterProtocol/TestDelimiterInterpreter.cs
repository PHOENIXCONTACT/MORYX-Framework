namespace Marvin.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    public class TestDelimiterInterpreter : EndDelimiterOnlyInterpreter
    {
        public static byte[] TestStartDelimiter = { 4, 2, 4, 2 };

        /// <summary>
        /// Byte sequence for start of message
        /// </summary>
        protected override byte[] StartDelimiter => TestStartDelimiter;
    }
}