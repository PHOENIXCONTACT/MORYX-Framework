// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Attribute used to decorate fields of binary payload objects
    /// </summary>
    public class PayloadPositionAttribute : Attribute
    {
        /// <summary>
        /// Initialize field with its position
        /// </summary>
        public PayloadPositionAttribute(int position)
        {
            Position = position;
        }

        /// <summary>
        /// Position of this property in the byte array
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Length of this field - for strings or collections
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Maximum length - for strings or collections
        /// </summary>
        public int MaxLength { get; set; }
    }
}
