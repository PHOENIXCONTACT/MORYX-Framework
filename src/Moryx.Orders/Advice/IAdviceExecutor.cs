// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Orders.Advice
{
    /// <summary>
    /// API for an advice executor. The executor applies an advice of pick parts or orders to an sub system
    /// </summary>
    public interface IAdviceExecutor : IConfiguredPlugin<AdviceExecutorConfig>
    {
        /// <summary>
        /// Indicator if the advice need user action or can be done automatically
        /// </summary>
        bool RequiresUserAction { get; }

        /// <summary>
        /// Advice an order
        /// </summary>
        /// <param name="operation">Operation of the advice</param>
        /// <param name="advice">Order advice information</param>
        Task<AdviceResult> Advice(Operation operation, OrderAdvice advice);

        /// <summary>
        /// Advice an pick part
        /// </summary>
        /// <param name="operation">Operation of the advice</param>
        /// <param name="advice">Pick part advice information</param>
        Task<AdviceResult> Advice(Operation operation, PickPartAdvice advice);
    }
}