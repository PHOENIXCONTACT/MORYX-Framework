namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Constraint for the context provided.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Checks if the context matches to the constraints.
        /// </summary>
        /// <returns>True: all is ok and valid, False: nothing is ok and not valid.</returns>
        bool Check(IConstraintContext context);
    }
}
