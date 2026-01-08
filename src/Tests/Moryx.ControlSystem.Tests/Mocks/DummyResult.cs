// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Tests
{
    public enum DummyResult
    {
        /// <summary>
        /// Production step was successful
        /// </summary>
        Done,

        /// <summary>
        /// Production step was not successful
        /// </summary>
        Failed,

        /// <summary>
        /// The activity could not be started at all.
        /// </summary>
        TechnicalFailure
    }
}