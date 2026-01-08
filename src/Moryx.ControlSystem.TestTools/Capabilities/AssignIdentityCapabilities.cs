// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.Capabilities
{
    /// <summary>
    /// Capabilities needed for AssignActivity
    /// </summary>
    [DataContract]
    public class AssignIdentityCapabilities : CapabilitiesBase
    {
        /// <summary>
        /// All <see cref="NumberingScheme"/> supported or required.
        /// </summary>
        [DataMember]
        public int[] Schemes { get; set; }

        /// <summary>
        /// Supported sources for identities
        /// </summary>
        [DataMember]
        public IdentitySource Source { get; set; }

        /// <summary>
        /// Create capabilities with list of provided or required schemes
        /// </summary>
        public AssignIdentityCapabilities(IdentitySource source, params int[] schemes)
        {
            Source = source;
            Schemes = schemes;
        }

        /// <inheritdoc />
        protected override bool ProvidedBy(ICapabilities provided)
        {
            // Check if the type of provided capabilities matches
            var casted = provided as AssignIdentityCapabilities;

            // Make sure all schemes are supported
            return casted != null && Schemes.All(casted.Schemes.Contains) && Source.HasFlag(casted.Source);
        }
    }
}
