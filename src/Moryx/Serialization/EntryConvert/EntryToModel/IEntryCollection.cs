// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Serialization
{
    /// <summary>
    /// Interface for all config entry collections
    /// </summary>
    public interface IEntryCollection
    {
        /// <summary>
        /// Export the current collection as list of config entries
        /// </summary>
        /// <returns></returns>
        List<Entry> ConfigEntries();
    }
}
