// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Interface for drivers that offer data input
    /// </summary>
    public interface IInputDriver<out TIn> : IDriver
    {
        /// <summary>
        /// Access to input values and events
        /// </summary>
        IInput<TIn> Input { get; }
    }

    /// <summary>
    /// Interface for drivers that offer data output
    /// </summary>
    public interface IOutputDriver<TOut> : IDriver
    {
        /// <summary>
        /// Access to data output
        /// </summary>
        IOutput<TOut> Output { get; }
    }

    /// <summary>
    /// Most basic driver interface for input and output of data
    /// This can be used to create less specific dependencies or simply as an additional interface
    /// </summary>
    public interface IInOutDriver<out TIn, TOut> : IInputDriver<TIn>, IOutputDriver<TOut>
    {
    }
}
