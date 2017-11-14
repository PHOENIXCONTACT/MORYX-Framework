namespace Marvin.Workflows
{
    /// <summary>
    /// Classification of the steps 
    /// </summary>
    public enum StepClassification
    {
        /// <summary>
        /// Task is executed and performs some sort of execution logic
        /// </summary>
        Execution,

        /// <summary>
        /// Step is internal and only alters the control flow (Split, Join, Conditional, Loop)
        /// </summary>
        ControlFlow,

        /// <summary>
        /// Step holds a subworkplan that is either executed or compiled
        /// </summary>
        Subworkplan,
    }
}