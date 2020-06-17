using System.Collections.Generic;
using System.Linq;
using Marvin.Workflows.Transitions;

namespace Marvin.Workflows
{
    internal static class WorkflowFactory
    {
        public static IWorkflow Instantiate(IWorkplan workplan, IWorkplanContext context)
        {
            // Prepare variables
            var places = new Dictionary<long, IPlace>();
            var transitions = new List<ITransition>();

            // Iterate over each step and find its connectors
            foreach (var step in workplan.Steps)
            {
                // Create transition
                var transition = step.CreateInstance(context);

                // Set inputs
                for (int i = 0; i < step.Inputs.Length; i++)
                {
                    transition.Inputs[i] = GetPlace(step.Inputs[i], places);
                }

                // Set outputs
                for (int i = 0; i < step.Outputs.Length; i++)
                {
                    transition.Outputs[i] = GetPlace(step.Outputs[i], places);
                }

                transitions.Add(transition);
            }

            return new SimpleWorkflow(workplan, places.Values.ToList(), transitions);
        }

        private static IPlace GetPlace(IConnector connector, IDictionary<long, IPlace> cache)
        {
            IPlace instance;
            if (cache.ContainsKey(connector.Id))
                instance = cache[connector.Id];
            else
            {
                instance = connector.CreateInstance();
                cache[connector.Id] = instance;
            }
            return instance;
        }
    }
}