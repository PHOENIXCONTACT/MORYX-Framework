using System;

namespace Marvin.AbstractionLayer.Drivers.DigitalIO
{
    /// <summary>
    /// API for inputs
    /// </summary>
    public interface IDigitalInputs
    {
        /// <summary>
        /// Read or write the value at his numerical index
        /// </summary>
        bool this[int index] { get; }

        /// <summary>
        /// Read or write the value at this named index
        /// </summary>
        bool this[string name] { get; }

        /// <summary>
        /// Event raised when an input has changed
        /// </summary>
        event EventHandler<InputChangedEventArgs> InputChanged;
    }

    /// <summary>
    /// Event args of a changed input
    /// </summary>
    public class InputChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Index of the value
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Optional name of the value
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Create a new <see cref="InputChangedEventArgs"/>
        /// </summary>
        public InputChangedEventArgs(int index, bool value) : this(index, string.Empty, value)
        {
        }

        /// <summary>
        /// Create a new <see cref="InputChangedEventArgs"/>
        /// </summary>
        public InputChangedEventArgs(int index, string name, bool value)
        {
            Index = index;
            Name = name;
            Value = value;
        }
    }
}