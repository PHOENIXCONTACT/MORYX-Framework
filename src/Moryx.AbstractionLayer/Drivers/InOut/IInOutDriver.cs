// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.InOut
{
    /// <summary>
    /// Most basic driver interface for input and output of data
    /// This can be used to create less specific dependencies or simply as an additional interface
    /// </summary>
    public interface IInOutDriver : IDriver
    {
        /// <summary>
        /// Access to input values and events
        /// </summary>
        IInput Input { get; }

        /// <summary>
        /// Access to data output
        /// </summary>
        IOutput Output { get; }
    }
}
