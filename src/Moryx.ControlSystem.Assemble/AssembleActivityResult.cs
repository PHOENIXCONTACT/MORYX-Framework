﻿using System.ComponentModel.DataAnnotations;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// Default results for all "Assemble Activities"
    /// </summary>
    public enum AssembleActivityResult
    {
        /// <summary>
        /// OK
        /// </summary>
        Success = 0,

        /// <summary>
        /// The part is defect and has been removed
        /// </summary>
        [Display(Name = "Ausschuss")]
        Failed = 1,

        /// <summary>
        /// A technical failure occured
        /// </summary>
        TechnicalError = 2,
    }
}
