// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Sockets.IntegrationTests
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
            return sysHeader != null && sysHeader.ClientIdx.Equals(_senderId);
        }

        /// <summary>
        /// Interpreter used by the protocol of this validator
        /// </summary>
        public IMessageInterpreter Interpreter => SystemTestMessageInterpreter.Instance;
    }
}
