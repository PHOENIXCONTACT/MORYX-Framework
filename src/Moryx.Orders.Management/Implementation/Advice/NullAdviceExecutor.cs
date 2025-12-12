// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Orders.Advice;

namespace Moryx.Orders.Management.Advice
{
    /// <summary>
    /// Default implementation for an advice executor. Will always raise errors
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IAdviceExecutor), Name = nameof(NullAdviceExecutor))]
    internal class NullAdviceExecutor : IAdviceExecutor
    {
        /// <inheritdoc />
        public Task InitializeAsync(AdviceExecutorConfig config, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<AdviceResult> AdviceAsync(Operation operation, OrderAdvice advice, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AdviceResult(advice, -1)
            {
                Message = "Advices are not allowed"
            });
        }

        /// <inheritdoc />
        public Task<AdviceResult> AdviceAsync(Operation operation, PickPartAdvice advice, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AdviceResult(advice, -1)
            {
                Message = "Advices are not allowed"
            });
        }
    }
}
