// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Orders.Advice;

namespace Moryx.Orders.Management.Advice
{
    /// <summary>
    /// Component which handles advices for an operation
    /// </summary>
    internal interface IAdviceManager : IPlugin
    {
        /// <summary>
        /// This will create an order advice for produced parts
        /// </summary>
        /// <param name="operationData">The operation for the advice</param>
        /// <param name="toteBoxNumber">Number of the ToteBox to advice</param>
        /// <param name="amount">Amount of produced parts</param>
        Task<AdviceResult> OrderAdvice(IOperationData operationData, string toteBoxNumber, int amount);

        /// <summary>
        /// This will create an pick part advice
        /// </summary>
        /// <param name="operationData">The operation for the advice</param>
        /// <param name="part">Pick part which should be adviced</param>
        /// <param name="toteBoxNumber">Number of the ToteBox to advice</param>
        /// <returns></returns>
        Task<AdviceResult> PickPartAdvice(IOperationData operationData, string toteBoxNumber, ProductPart part);
    }
}
