// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx
{
    /// <summary>
    /// Common interface for all business objects containing a database ID
    /// </summary>
    public interface IPersistentObject
    {
        /// <summary>
        /// Id of this object
        /// </summary>
        long Id { get; set; }
    }
}
