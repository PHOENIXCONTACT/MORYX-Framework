// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.Setups
{
    /// <summary>
    /// Response object
    /// </summary>
    public class SetupEvaluation
    {
        /// <summary>
        /// Flag if the evaluation has not determined the need for a setup
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// Type of setup
        /// </summary>
        public SetupClassification Classification { get; }

        /// <summary>
        /// Default constructor to set the important <see cref="Required"/>-flag
        /// </summary>
        public SetupEvaluation(bool required)
        {
            Required = required;
        }

        /// <summary>
        /// Create setup evaluation for type
        /// </summary>
        public SetupEvaluation(SetupClassification classification)
        {
            Required = true;
            Classification = classification;
        }

        /// <summary>
        /// Value is converted to <see cref="Required"/> flag
        /// </summary>
        public static implicit operator SetupEvaluation(bool argument) => new(argument);

        /// <summary>
        /// Convert <see cref="SetupClassification"/> to <see cref="SetupEvaluation"/>
        /// </summary>
        public static implicit operator SetupEvaluation(SetupClassification classification) => new(classification);

        /// <summary>
        /// Special setup evaluation that indicates a capabilities reservation
        /// </summary>
        public class Reservation : SetupEvaluation
        {
            /// <summary>
            /// Capabilities that are reserved or released
            /// </summary>
            public ICapabilities Capabilities { get; }

            /// <summary>
            /// Setup evaluation for capabilities. Reservations are always automatic
            /// </summary>
            public Reservation(ICapabilities capabilities, SetupClassification classification) : base(classification)
            {
                Capabilities = capabilities;
            }
        }

        /// <summary>
        /// Special setup change that indicates a capabilities change
        /// </summary>
        public class Change : SetupEvaluation
        {
            /// <summary>
            /// Capabilities that are changed
            /// </summary>
            public ICapabilities CurrentCapabilities { get; }

            /// <summary>
            /// Capabilities that are present AFTER the change
            /// </summary>
            public ICapabilities TargetCapabilities { get; }

            /// <summary>
            /// Setup evaluation for type and capabilities
            /// </summary>
            public Change(SetupClassification classification, ICapabilities current, ICapabilities target) : base(classification)
            {
                CurrentCapabilities = current;
                TargetCapabilities = target;
            }
        }

        /// <summary>
        /// Provide capabilities
        /// </summary>
        public static SetupEvaluation Provide(ICapabilities capabilities) => new Change(SetupClassification.Unspecified, NullCapabilities.Instance, capabilities);

        /// <summary>
        /// Provide capabilities
        /// </summary>
        public static SetupEvaluation Provide(ICapabilities capabilities, ICapabilities current) => new Change(SetupClassification.Unspecified, current, capabilities);

        /// <summary>
        /// Provide capabilities
        /// </summary>
        public static SetupEvaluation Provide(ICapabilities capabilities, SetupClassification classification) => new Change(classification, NullCapabilities.Instance, capabilities);

        /// <summary>
        /// Provide capabilities
        /// </summary>
        public static SetupEvaluation Provide(ICapabilities capabilities, ICapabilities current, SetupClassification classification) => new Change(classification, current, capabilities);

        /// <summary>
        /// Remove capabilities
        /// </summary>
        public static SetupEvaluation Remove(ICapabilities capabilities) => new Change(SetupClassification.Unspecified, capabilities, NullCapabilities.Instance);

        /// <summary>
        /// Remove capabilities
        /// </summary>
        public static SetupEvaluation Remove(ICapabilities capabilities, ICapabilities targetCapabilities) => new Change(SetupClassification.Unspecified, capabilities, targetCapabilities);

        /// <summary>
        /// Remove capabilities
        /// </summary>
        public static SetupEvaluation Remove(ICapabilities capabilities, SetupClassification classification) => new Change(classification, capabilities, capabilities);

        /// <summary>
        /// Remove capabilities
        /// </summary>
        public static SetupEvaluation Remove(ICapabilities capabilities, ICapabilities targetCapabilities, SetupClassification classification) => new Change(classification, capabilities, targetCapabilities);

        /// <summary>
        /// Reserve capabilities and provide setup information for the current recipe without changing them
        /// </summary>
        public static SetupEvaluation Reserve(ICapabilities capabilities) => new Reservation(capabilities, SetupClassification.Unspecified);

        /// <summary>
        /// Reserve capabilities and provide setup information for the current recipe without changing them
        /// </summary>
        public static SetupEvaluation Reserve(ICapabilities capabilities, SetupClassification classification) => new Reservation(capabilities, classification);

        /// <summary>
        /// Release capabilities and indicate the provided setup information is no longer valid
        /// </summary>
        public static SetupEvaluation Release(ICapabilities capabilities) => new Reservation(capabilities, SetupClassification.Unspecified);

        /// <summary>
        /// Release capabilities and indicate the provided setup information is no longer valid
        /// </summary>
        public static SetupEvaluation Release(ICapabilities capabilities, SetupClassification classification) => new Reservation(capabilities, classification);
    }
}
