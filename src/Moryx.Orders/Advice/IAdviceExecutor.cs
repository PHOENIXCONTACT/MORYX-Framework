// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Orders.Advice
{
    /// <summary>
    /// API for an advice executor. The executor applies an advice of pick parts or orders to an sub system
    /// </summary>
    public interface IAdviceExecutor : IAsyncConfiguredPlugin<AdviceExecutorConfig>
    {
        /// <summary>
        /// Advice an order
        /// </summary>
        /// <param name="operation">Operation of the advice</param>
        /// <param name="advice">Order advice information</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        Task<AdviceResult> AdviceAsync(Operation operation, OrderAdvice advice, CancellationToken cancellationToken);

        /// <summary>
        /// Advice an pick part
        /// </summary>
        /// <param name="operation">Operation of the advice</param>
        /// <param name="advice">Pick part advice information</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        Task<AdviceResult> AdviceAsync(Operation operation, PickPartAdvice advice, CancellationToken cancellationToken);
    }
}
