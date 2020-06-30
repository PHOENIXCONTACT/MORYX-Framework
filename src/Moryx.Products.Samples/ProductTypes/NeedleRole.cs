// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Samples
{
    /// <summary>
    /// Role of the referenced <see cref="NeedleType"/> in the <see cref="WatchType"/>
    /// </summary>
    public enum NeedleRole
    {
        /// <summary>
        /// Needle shows the hours
        /// </summary>
        Hours,

        /// <summary>
        /// Needle shows the minutes
        /// </summary>
        Minutes,

        /// <summary>
        /// Needle shows the seconds
        /// </summary>
        Seconds,

        /// <summary>
        /// Needle is used for a dedicated stopwatch
        /// </summary>
        Stopwatch
    }
}
