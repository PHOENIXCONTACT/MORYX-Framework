// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders
{
    /// <summary>
    /// Base class to provide operation information for beginning an operation or reporting
    /// </summary>
    public abstract class OperationInfo
    {
        /// <summary>
        /// Current success count of the operation
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Current failure count of the operation
        /// </summary>
        public int ScrapCount { get; set; }
    }
}