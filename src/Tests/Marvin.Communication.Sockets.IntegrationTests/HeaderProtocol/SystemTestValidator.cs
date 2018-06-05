namespace Marvin.Communication.Sockets.IntegrationTests
{
    public class SystemTestValidator : IMessageValidator
    {
        private readonly int _senderId;
        public SystemTestValidator(int senderId)
        {
            _senderId = senderId;
        }

        /// <inheritdoc />
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
        public IMessageInterpreter Interpreter => SystemTestMessageInterpreter.Instance;
    }
}
