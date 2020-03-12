// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Workflows
{
    /// <summary>
    /// A Workplan's state
    /// </summary>
    public enum WorkplanState
    {
        /// <summary>
        /// The workplan is new and may be used for test jobs only.
        /// </summary>
        New = 0,

        /// <summary>
        /// The workplan is released and may be used for regular jobs now.
        /// </summary>
        Released = 1,

        /// <summary>
        /// The workplan is revoked and must not be used anymore.
        /// </summary>
        Revoked = 2
    }
}
