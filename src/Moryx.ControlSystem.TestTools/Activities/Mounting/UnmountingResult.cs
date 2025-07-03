using Moryx.ControlSystem.VisualInstructions;
using System.ComponentModel.DataAnnotations;

namespace Moryx.ControlSystem.TestTools.Activities
{
    /// <summary>
    /// Result enum representing the results of unmounting activities
    /// </summary>
    public enum UnmountingResult
    {
        /// <summary>
        /// Article was unmounted from the carrier
        /// </summary>
        Removed = 0,

        /// <summary>
        /// Article was removed but shows visual deficits
        /// </summary>
        Faulty = 1,

        /// <summary>
        /// The activity could not be started at all.
        /// </summary>
        TechnicalFailure = 2
    }
}