﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Collections.Specialized;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Factory
{
    /// <summary>
    /// Location inside the factory
    /// </summary>
    public interface ILocation : IResource
    {
        /// <summary>
        /// Image of the location
        /// </summary>
        string Image { get; set; }

        /// <summary>
        /// Position of the location
        /// </summary>
        Position Position { get; set; }

        /// <summary>
        /// Transport paths that are going out of this location
        /// </summary>
        IEnumerable<ITransportPath> Origins { get; }

        /// <summary>
        /// Transport paths that are coming to this location
        /// </summary>
        IEnumerable<ITransportPath> Destinations { get; }
    }
}