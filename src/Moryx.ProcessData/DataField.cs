// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData
{
    /// <summary>
    /// The field used for holding the process data
    /// </summary>
    public class DataField
    {
        /// <summary>
        /// Name of the sample
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value of this sample
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="DataField"/>.
        /// </summary>
        public DataField(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}