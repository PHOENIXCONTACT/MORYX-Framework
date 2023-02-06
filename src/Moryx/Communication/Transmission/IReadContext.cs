// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Context object passed between 
    /// </summary>
    public interface IReadContext
    {
        /// <summary>
        /// Current index in the payload array
        /// </summary>
        int CurrentIndex { get; }

        /// <summary>
        /// Current message buffer
        /// </summary>
        byte[] ReadBuffer { get; }

        /// <summary>
        /// Number of bytes to read
        /// </summary>
        int ReadSize { get; }
    }
}
