// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    public class TestDelimiterValidator : IMessageValidator
    {
        public bool Validate(BinaryMessage message) => true;

        public IMessageInterpreter Interpreter => new TestDelimiterInterpreter();
    }
}
