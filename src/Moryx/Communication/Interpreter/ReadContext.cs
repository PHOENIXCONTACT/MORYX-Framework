// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Base class for tcp read context classes
    /// </summary>
    public abstract class ReadContext : IReadContext
    {
        /// <summary>
        /// Current index in the payload array
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// Current message buffer
        /// </summary>
        public byte[] ReadBuffer { get; set; }

        /// <summary>
        /// Number of bytes to read
        /// </summary>
        public int ReadSize { get; set; }
    }
}
