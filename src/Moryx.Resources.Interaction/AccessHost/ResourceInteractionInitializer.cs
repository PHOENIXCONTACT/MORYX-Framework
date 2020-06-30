// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Interaction
{
    /// <summary>
    /// AccessHost initializer to create the default webservice on startup
    /// </summary>
    [ResourceInitializer(nameof(ResourceInteractionInitializer))]
    public class ResourceInteractionInitializer : ResourceInitializerBase
    {
        /// <inheritdoc />
        public override string Name => "ResourceInteractionOnly";

        /// <inheritdoc />
        public override string Description => "Creates a default interaction host";

        /// <inheritdoc />
        public override IReadOnlyList<Resource> Execute(IResourceGraph graph)
        {
            var interactionHost = graph.Instantiate<ResourceInteractionHost>();
            return new[] { interactionHost };
        }
    }
}
