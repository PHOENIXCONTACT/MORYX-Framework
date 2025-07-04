// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
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
        public bool RequiresUserAction => false;

        /// <inheritdoc />
        public void Initialize(AdviceExecutorConfig config)
        {
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
        }

        /// <inheritdoc />
        public Task<AdviceResult> Advice(Operation operation, OrderAdvice advice)
        {
            return Task.FromResult(new AdviceResult(advice, -1)
            {
                Message = "Advices are not allowed"
            });
        }

        /// <inheritdoc />
        public Task<AdviceResult> Advice(Operation operation, PickPartAdvice advice)
        {
            return Task.FromResult(new AdviceResult(advice, -1)
            {
                Message = "Advices are not allowed"
            });
        }
    }
}
