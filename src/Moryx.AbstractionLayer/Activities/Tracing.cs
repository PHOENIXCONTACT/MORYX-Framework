// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Activity trace information
    /// </summary>
    [DataContract]
    public class Tracing
    {
        /// <summary>
        /// The time when this activity was started.
        /// </summary>
        public DateTime? Started { get; set; }

        /// <summary>
        /// The time when this activity was finished.
        /// </summary>
        public DateTime? Completed { get; set; }

        /// <summary>
        /// Optional tracing text for errors or information
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Contains the error code that is associated with the error that caused e.g. an activity failure
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Generic progress information
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Resource that executed the activity
        /// </summary>
        public long ResourceId { get; set; }

        ///
        // ReSharper disable once InconsistentNaming <-- too cool to rename :P
        public Sparta Transform<Sparta>() where Sparta
            : Tracing, new()
        {
            if (this is Sparta)
                return this as Sparta;

            var replacement = new Sparta();
            var replacementType = typeof(Sparta);
            var sharedProperties = GetType().GetProperties()
                // ReSharper disable once PossibleNullReferenceException
                .Where(p => p.DeclaringType.IsAssignableFrom(replacementType));

            foreach (var property in sharedProperties)
            {
                var value = property.GetValue(this);
                if (property.CanWrite)
                    property.SetValue(replacement, value);
            }

            return replacement;
        }
    }
}
