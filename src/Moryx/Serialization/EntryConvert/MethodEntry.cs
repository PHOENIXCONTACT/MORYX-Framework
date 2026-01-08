// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Serialization
{
    /// <summary>
    /// Model that represents a public method on an object callable from the UI
    /// </summary>
    [DataContract]
    public class MethodEntry : ICloneable
    {
        /// <summary>
        /// Name of the method
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Flag if this structure represents a server side constructor
        /// </summary>
        public bool IsConstructor { get; set; }

        /// <summary>
        /// Display name for the button
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of the method
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Arguments of the
        /// </summary>
        [DataMember]
        public Entry Parameters { get; set; }

        /// <summary>
        /// True if the method is awaitable by <see cref="Task"/>
        /// </summary>
        [DataMember]
        public bool IsAsync { get; set; }

        /// <see cref="ICloneable"/>
        public object Clone()
        {
            return Clone(true);
        }

        /// <summary>
        /// Method to create a deep or shallow copy of this object
        /// </summary>
        public MethodEntry Clone(bool deep)
        {
            // All value types can be simply copied
            var copy = new MethodEntry
            {
                Name = Name,
                DisplayName = DisplayName,
                Description = Description,
                IsAsync =  IsAsync
            };

            if (deep)
            {
                if (Parameters != null)
                {
                    copy.Parameters = Parameters.Clone(deep);
                }
            }
            else
            {
                // In a shallow clone only references are copied
                copy.Parameters = Parameters;
            }

            return copy;
        }
    }
}
