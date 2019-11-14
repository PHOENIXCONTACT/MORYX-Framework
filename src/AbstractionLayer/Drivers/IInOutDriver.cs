using System;

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

    /// <summary>
    /// Interface for reading input
    /// </summary>
    public interface IInput<out TIn>
    {
        /// <summary>
        /// Access flags for the input
        /// </summary>
        SupportedAccess Access { get; }

        /// <summary>
        /// Single value input
        /// Only available for <see cref="SupportedAccess.Single"/>
        /// </summary>
        TIn Value { get; }

        /// <summary>
        /// Index based input
        /// Only available for <see cref="SupportedAccess.Index"/>
        /// </summary>
        TIn this[int index] { get; }

        /// <summary>
        /// Key based input
        /// Only available for <see cref="SupportedAccess.Key"/>
        /// </summary>
        TIn this[string key] { get; }

        /// <summary>
        /// Event raised when any input value changed
        /// </summary>
        event EventHandler<EventArgs> InputChanged;
    }

    /// <summary>
    /// Interface for writing output
    /// </summary>
    public interface IOutput<TOut>
    {
        /// <summary>
        /// Access flags for the output
        /// </summary>
        SupportedAccess Access { get; }

        /// <summary>
        /// Single value output
        /// Only available for <see cref="SupportedAccess.Single"/>
        /// </summary>
        TOut Value { get; set;  }

        /// <summary>
        /// Index based output
        /// Only available for <see cref="SupportedAccess.Index"/>
        /// </summary>
        TOut this[int index] { get; set; }

        /// <summary>
        /// Key based output
        /// Only available for <see cref="SupportedAccess.Key"/>
        /// </summary>
        TOut this[string key] { get; set; }
    }
}