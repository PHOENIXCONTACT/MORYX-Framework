// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Sockets.IntegrationTests
{
    public class SystemTestMessageInterpreter : HeaderMessageInterpreter<SystemTestHeader>
    {
        private static SystemTestMessageInterpreter _instance;
        public static SystemTestMessageInterpreter Instance => _instance ?? (_instance = new SystemTestMessageInterpreter());

        public SystemTestMessageInterpreter()
        {
            HeaderSize = new SystemTestHeader().ToBytes().Length;
        }

        /// <summary>
        /// Size of the header
        /// </summary>
        protected override int HeaderSize { get; }
    }
}
