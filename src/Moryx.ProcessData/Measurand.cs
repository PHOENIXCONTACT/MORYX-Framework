// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData
{
    /// <summary>
    /// A measurand is a collector for measurements. It has a list of measurements.
    /// </summary>
    public class Measurand
    {
        /// <summary>
        /// Unique identifier of this measurand
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Name of this measurand
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of measurements connected to this measurement
        /// </summary>
        public IList<Measurement> Measurements { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Measurand"/>.
        /// </summary>
        public Measurand(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Measurand"/>.
        /// </summary>
        public Measurand()
        {
            Id = Guid.NewGuid();
            Measurements = new List<Measurement>();
        }
    }
}