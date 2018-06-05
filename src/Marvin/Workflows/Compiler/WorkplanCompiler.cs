using System.Collections.Generic;
using System.Linq;

namespace Marvin.Workflows.Compiler
{
    /// <summary>
    /// Compiler used to create simpilyfied workplans for embedded devices
    /// 
    /// This code was optimized was performance. Any structure changes should be validated with the benchmark.
    /// Anything worse than O(n) is inacceptable
    /// Current values for Release build: 
    /// *  250 steps :  5ms
    /// * 1000 steps : 18ms
    /// * 2000 steps : 37ms
    /// </summary>
    internal static class WorkplanCompiler
    {
        /// <summary>
        /// Compile the workplan into a list of steps and a decision matrix
        /// </summary>
        internal static CompiledWorkplan<TStep> Compile<TStep>(IWorkplan workplan, IWorkplanContext context, ICompiler<TStep> compiler)
            where TStep : CompiledTransition
        {
            // Preparations
            // Calculate the basic metrics to avoid unnecessary double execution
            int stepCount = 0, maxOutputs = 0;
            foreach (var step in workplan.Steps)
            {
                stepCount++;
                if (step.Outputs.Length > maxOutputs)
                    maxOutputs = step.Outputs.Length;
            }
            var outputs = workplan.Connectors.Where(c => c.Classification.HasFlag(NodeClassification.Exit)).ToArray();
            var totalStepCount = stepCount + outputs.Length;

            // Prepare fields of compiled object
            var steps = new TStep[totalStepCount];
            var matrix = new int[stepCount, maxOutputs];

            #region Compile the plan

            int index = 0;
            var unlinkedSteps = new List<TStep>(16);  
            // Good compromise between memory waste and performance for average sized workplans
            // Loop over all steps and create a flat representation of the step. Make sure to only iterate over the collection once.
            // Each step also carries references to its output connectors --> Look ahead
            // To create the mapping each step must check its inputs against previously unmapped outputs --> Look back
            foreach (var workplanStep in OrderedSteps(workplan))
            {
                // Create step
                var transition = workplanStep.CreateInstance(context);
                var step = compiler.CompileTransition(transition);
                step.Id = index + 1;
                step.SourceId = transition.Id;

                // Save follow up connectors
                step.OutputRelations = workplanStep.Outputs.Select(o => new OutputRelation(o.Id)).ToArray();
                unlinkedSteps.Add(step);

                // Link this step to previous steps
                foreach (var outputRelation in unlinkedSteps.SelectMany(u => u.OutputRelations).Where(o => !o.IsMapped))
                {
                    var input = workplanStep.Inputs.FirstOrDefault(i => i.Id == outputRelation.ConnectorId);
                    if (input != null)
                        outputRelation.MapTo(step.Id);
                }
                // Remove a step as soon as all exits are linked
                unlinkedSteps.RemoveAll(unlinked => unlinked.OutputRelations.All(or => or.IsMapped));

                // Add to step list
                steps[index] = step;

                index++;
            }

            // Save index of first output
            var firstOutput = index;

            // Compile outputs to steps as well and add to end of array
            foreach (var exit in outputs)
            {
                // Create output
                var output = compiler.CompileResult(exit);
                output.Id = index + 1;
                output.SourceId = exit.Id;

                // Link this output to all unlinked steps
                foreach (var outputRelation in unlinkedSteps.SelectMany(u => u.OutputRelations).Where(o => !o.IsMapped))
                {
                    if (outputRelation.ConnectorId == exit.Id)
                        outputRelation.MapTo(output.Id);
                }
                // Remove a step as soon as all exits are linked
                unlinkedSteps.RemoveAll(unlinked => unlinked.OutputRelations.All(or => or.IsMapped));

                // Add to step list
                steps[index++] = output;
            }

            #endregion

            #region Compile the matrix

            // Use all connections to create the 2 dimensional decision array by iterating over all steps and setting
            // the follow up step for each output
            for (index = 0; index < stepCount; index++)
            {
                var step = steps[index];
                for (int outNumber = 0; outNumber < step.OutputRelations.Length; outNumber++)
                {
                    matrix[index, outNumber] = step.OutputRelations[outNumber].StepId;
                }
            }

            #endregion

            return new CompiledWorkplan<TStep>
            {
                Steps = steps,
                FirstOutput = firstOutput,
                DecisionMatrix = matrix
            };
        }

        // Create ordered list of steps for the compiler
        private static IEnumerable<IWorkplanStep> OrderedSteps(IWorkplan workplan)
        {
            // Buffer collections
            var steps = workplan.Steps.ToList();
            var openOutputs = workplan.Connectors.Where(con => con.Classification.HasFlag(NodeClassification.Entry)).ToList();

            // Make sure the order of steps roughly matches the execution order of the plan
            while (steps.Count > 0)
            {
                var postponedSteps = new List<IWorkplanStep>(8);
                foreach (var step in steps)
                {
                    var inputMatch = step.Inputs.FirstOrDefault(openOutputs.Contains);
                    if (inputMatch == null)
                    {
                        postponedSteps.Add(step);
                    }
                    else
                    {
                        openOutputs.Remove(inputMatch);
                        openOutputs.AddRange(step.Outputs);
                        yield return step;
                    }
                }
                steps = postponedSteps;
            }
        }
    }
}