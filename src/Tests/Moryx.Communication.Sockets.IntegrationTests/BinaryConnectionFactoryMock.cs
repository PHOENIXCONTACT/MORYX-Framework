// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Logging;

namespace Moryx.Communication.Sockets.IntegrationTests
{
    public class BinaryConnectionFactoryMock : IBinaryConnectionFactory
    {
        private readonly IModuleLogger _logger;

        public BinaryConnectionFactoryMock()
        {
            _logger = new ModuleLogger("Dummy", new NullLoggerFactory());
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
