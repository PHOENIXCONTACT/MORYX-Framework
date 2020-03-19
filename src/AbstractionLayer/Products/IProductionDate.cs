// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for product instances with production date
    /// </summary>
    public interface IProductionDate
    {
        /// <summary>
        /// Date of production
        /// </summary>
        DateTime ProductionDate { get; set; }
    }
}
