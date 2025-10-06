// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Modules
{
    /// <summary>
    /// Base interface for all components collaborating within an <see cref="IModule"/> composition.
    /// For increased flexibility and extensibility modules should be designed as compositions of exchangeable plugins.
    /// Application specific behaviour and patterns like strategy or CoR pattern should include plugins.
    /// Tailored to the different requirements <see cref="IAsyncPlugin"/> come in different variations.
    /// To provide fully restartable modules each plugin must be fully disposable.
    /// </summary>
    public interface IAsyncPlugin
    {
        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        Task Start();

        /// <summary>
        /// Stops internal execution of active and/or periodic functionality.
        /// </summary>
        Task Stop();
    }
}