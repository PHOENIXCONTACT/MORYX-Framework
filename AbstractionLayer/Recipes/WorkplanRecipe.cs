using System.Collections.Generic;
using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Recipe which additionaly contains a workplan
    /// </summary>
    public class WorkplanRecipe : Recipe, IWorkplanRecipe
    {
        /// 
        public override string Type => nameof(WorkplanRecipe);

        /// <summary>
        /// Prepare recipe by filling DisabledSteps and TaskAssignment properties
        /// </summary>
        public WorkplanRecipe()
        {
            DisabledSteps = new List<long>();
            TaskAssignment = new Dictionary<long, long>();
        }

        /// <summary>
        /// Clone a workplan recipe
        /// </summary>
        protected WorkplanRecipe(WorkplanRecipe source)
            : base(source)
        {
            Workplan = source.Workplan;
            DisabledSteps = source.DisabledSteps;
            TaskAssignment = source.TaskAssignment;
        }

        /// <inheritdoc />
        public IWorkplan Workplan { get; set; }

        /// <inheritdoc />
        public ICollection<long> DisabledSteps { get; }

        /// <inheritdoc />
        public IDictionary<long, long> TaskAssignment { get; }

        /// <inheritdoc />
        public override IRecipe Clone()
        {
            return new WorkplanRecipe(this);
        }
    }
}