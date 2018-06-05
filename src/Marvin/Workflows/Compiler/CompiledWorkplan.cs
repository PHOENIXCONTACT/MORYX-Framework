using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marvin.Workflows.Compiler
{
    /// <summary>
    /// Small POCO created by compiling a workplan object
    /// </summary>
    public class CompiledWorkplan<TStep>
        where TStep : CompiledTransition
    {
        /// <summary>
        /// Flat list of all steps and outputs
        /// </summary>
        public TStep[] Steps { get; set; }

        /// <summary>
        /// Index of the last step. Marks the beginning of output steps in the array
        /// </summary>
        public int FirstOutput { get; set; }

        /// <summary>
        /// Machine compiled decision matrix
        /// </summary>
        public int[,] DecisionMatrix { get; set; }
    }
}
