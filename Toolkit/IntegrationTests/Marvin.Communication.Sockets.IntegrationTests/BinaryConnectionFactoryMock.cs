using Marvin.Logging;
using Marvin.TestTools.UnitTest;

namespace Marvin.Communication.Sockets.IntegrationTests
{
    public class BinaryConnectionFactoryMock : IBinaryConnectionFactory
    {
        private readonly IModuleLogger _logger;

        public BinaryConnectionFactoryMock()
        {
            _logger = new DummyLogger();
        }

        public IBinaryConnection Create(BinaryConnectionConfig config, IMessageValidator validator)
        {
            IBinaryConnection connection = null;
            if (config is TcpClientConfig)
                connection = new TcpClientConnection(validator) { Logger = _logger };

            if (config is TcpListenerConfig)
                connection = new TcpListenerConnection(validator) { Logger = _logger };

            connection?.Initialize(config);

            return connection;
        }

        public void Destroy(IBinaryConnection instance)
        {
        }
    }
}