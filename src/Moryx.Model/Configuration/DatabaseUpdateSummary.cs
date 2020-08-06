// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Result object created from running a database update
    /// </summary>
    public class DatabaseUpdateSummary
    {
        /// <summary>
        /// Flag that any updates were performed
        /// </summary>
        public bool WasUpdated { get; set; }

        /// <summary>
        /// All updates that were executed
        /// </summary>
        public DatabaseUpdate[] ExecutedUpdates { get; set; }
    }
}
