namespace Marvin.Communication.Sockets.IntegrationTests
{
    public class SystemTestValidator : IMessageValidator
    {
        private readonly int _senderId;
        public SystemTestValidator(int senderId)
        {
            _senderId = senderId;
        }

        /// <summary>
        /// Validate the message
        /// </summary>
        public bool Validate(BinaryMessage message)
        {
            var sysHeader = ((BinaryMessage<SystemTestHeader>)message).Header;

            if (sysHeader == null)
            {
                return false;
            }

            return sysHeader.ClientIdx.Equals(_senderId);
        }

        /// <summary>
        /// Interpreter used by the protocol of this validator
        /// </summary>
        public IMessageInterpreter Interpreter { get { return SystemTestMessageInterpreter.Instance; } }
    }

    public class SystemTestMessageInterpreter : HeaderMessageInterpreter<SystemTestHeader>
    {
        private static SystemTestMessageInterpreter _instance;
        public static SystemTestMessageInterpreter Instance
        {
            get { return _instance ?? (_instance = new SystemTestMessageInterpreter()); }
        }

        public SystemTestMessageInterpreter()
        {
            _headerSize = new SystemTestHeader().ToBytes().Length;
        }

        private readonly int _headerSize;
        /// <summary>
        /// Size of the header
        /// </summary>
        protected override int HeaderSize
        {
            get { return _headerSize; }
        }
    }
}
