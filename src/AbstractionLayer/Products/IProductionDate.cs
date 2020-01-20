using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for articles with production date
    /// </summary>
    public interface IProductionDate
    {
        /// <summary>
        /// Date of production
        /// </summary>
        DateTime ProductionDate { get; set; }
    }
}