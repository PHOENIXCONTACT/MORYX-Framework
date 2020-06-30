// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Configuration
{
    internal interface IPersistentConfig
    {
        /// <summary>
        /// Indicates if the config should be persisted if not changed
        /// </summary>
        bool PersistDefaultConfig { get; }
    }
}
