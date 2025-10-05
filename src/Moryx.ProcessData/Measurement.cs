// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.ProcessData
{
    /// <summary>
    /// The measurement acts as a container for tags, fields and the time.
    /// The measurement name is the description of the data. It is divided into **fields** and **tags**.
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Name of the measurand
        /// </summary>
        public string Measurand { get; }

        /// <summary>
        /// TimeStamp of the measurement
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// Collection of fields of this measurement
        /// </summary>
        public IList<DataField> Fields { get; }

        /// <summary>
        /// Collection of tags of this measurement
        /// </summary>
        public IList<DataTag> Tags { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Measurement"/>.
        /// </summary>
        public Measurement(string measurand) : this(measurand, DateTime.Now)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Measurement"/>.
        /// </summary>
        public Measurement(string measurand, DateTime timeStamp)
        {
            Measurand = measurand;
            TimeStamp = timeStamp;
            Fields = new List<DataField>();
            Tags = new List<DataTag>();
        }

        /// <summary>
        /// Shortcut to add a data field
        /// </summary>
        public void Add(DataField field) => Fields.Add(field);

        /// <summary>
        /// Shortcut to add a data tag
        /// </summary>
        public void Add(DataTag tag) => Tags.Add(tag);
    }
}
