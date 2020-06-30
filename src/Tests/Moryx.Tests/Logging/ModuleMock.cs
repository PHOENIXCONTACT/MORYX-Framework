// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.Tests.Logging
{
    public class ModuleMock : ILoggingHost
    {
        public string Name { get { return GetType().Name; } }

        public IModuleLogger Logger { get; set; }
    }
}
