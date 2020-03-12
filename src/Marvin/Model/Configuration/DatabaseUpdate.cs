// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Model
{
    /// <inheritdoc />
    public class DatabaseUpdate : IDatabaseUpdate
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public int From { get; set; }

        /// <inheritdoc />
        public int To { get; set; }
    }
}
