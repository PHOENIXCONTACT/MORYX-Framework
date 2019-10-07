using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Base class for tun modes
    /// </summary>
    /// <typeparam name="TOptions">Type of options parsed for this run mode</typeparam>
    public abstract class RunModeBase<TOptions> : IRunMode where TOptions : RuntimeOptions
    {
        #region Dependencies

        /// <inheritdoc />
        public IModuleManager ModuleManager { get; set; }

        #endregion

        /// <summary>
        /// Typed options for this run mode
        /// </summary>
        public TOptions Options { get; private set; }

        /// <inheritdoc />
        public virtual void Setup(RuntimeOptions args)
        {
            Options = (TOptions) args;
        }

        /// <inheritdoc />
        public abstract RuntimeErrorCode Run();
    }
}