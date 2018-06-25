namespace Marvin.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    public class TestDelimiterValidator : IMessageValidator
    {
        public bool Validate(BinaryMessage message) => true;

        public IMessageInterpreter Interpreter => new TestDelimiterInterpreter();
    }
}
