// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    public class TestDelimiterValidator : IAdvancedMessageValidator
    {
        public bool Validate(BinaryMessage message) => throw new Exception("Should not be called");

        public IMessageInterpreter Interpreter => new TestDelimiterInterpreter();

        public bool Validate(BinaryMessage message, bool initialMessage) => true;
    }
}
