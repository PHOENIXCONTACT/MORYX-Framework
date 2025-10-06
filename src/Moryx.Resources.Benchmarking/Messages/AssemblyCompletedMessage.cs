// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Resources.Benchmarking.Messages
{
    /// <summary>
    /// Message specific to complete complete an activity
    /// </summary>
    public class AssemblyCompletedMessage
    {
        /// <summary>
        /// Result of the completed activity, that can be mapped to its enum value
        /// </summary>
        public int Result { get; set; }
    }
}

