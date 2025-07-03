using Moryx.AbstractionLayer;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// Common interface for <see cref="AssembleActivity"/> and <see cref="AssembleActivity{TParam}"/>
    /// </summary>
    public interface IAssembleActivity : IActivity<AssembleParameters>
    {
    }
}