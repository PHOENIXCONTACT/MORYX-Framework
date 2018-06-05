namespace Marvin.Workflows.Compiler
{
    /// <summary>
    /// Strategy used to compile a step from the workplan
    /// </summary>
    /// <typeparam name="TStep"></typeparam>
    public interface ICompiler<out TStep>
        where TStep : CompiledTransition
    {
        /// <summary>
        /// Compile a workplanstep
        /// </summary>
        TStep CompileTransition(ITransition transition);

        /// <summary>
        /// Compile a result connector to an outfeed step
        /// </summary>
        TStep CompileResult(IConnector output);
    }
}