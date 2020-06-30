// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
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
