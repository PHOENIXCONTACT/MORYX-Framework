// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Products
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
