// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Orders.Advice;

namespace Moryx.Orders.Management.Advice
{
    [Plugin(LifeCycle.Singleton, typeof(IAdviceManager))]
    internal class AdviceManager : IAdviceManager
    {
        #region Dependencies

        /// <summary>
        /// Executor of advices. Strategy to replace the behavior.
        /// </summary>
        public IAdviceExecutor AdviceExecutor { get; set; }

        /// <summary>
        /// Configuration of the module
        /// </summary>
        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        /// <inheritdoc />
        public async Task StartAsync()
        {
            await AdviceExecutor.InitializeAsync(ModuleConfig.Advice.AdviceExecutor);
            await AdviceExecutor.StartAsync();
        }

        /// <inheritdoc />
        public Task StopAsync()
        {
            return AdviceExecutor.StopAsync();
        }

        /// <inheritdoc />
        public Task<AdviceResult> OrderAdvice(IOperationData operationData, string toteBoxNumber, int amount)
        {
            var advice = new OrderAdvice(toteBoxNumber, amount);
            return Advice(operationData, advice);
        }

        /// <inheritdoc />
        public Task<AdviceResult> PickPartAdvice(IOperationData operationData, string toteBoxNumber, ProductPart part)
        {
            var advice = new PickPartAdvice(part, toteBoxNumber);
            if (part.StagingIndicator != StagingIndicator.PickPart)
            {
                return Task.FromResult(new AdviceResult(advice, -1)
                {
                    Message = "Only pick parts are allowed for advice."
                });
            }

            return Advice(operationData, advice);
        }

        private async Task<AdviceResult> Advice<TAdvice>(IOperationData operationData, TAdvice advice)
            where TAdvice : OperationAdvice
        {
            AdviceResult result;

            if (advice is OrderAdvice && !ModuleConfig.Advice.UseAdviceExecutorForOrderAdvice)
            {
                result = new AdviceResult(advice);
            }
            else
            {
                result = await AdviceExecutor.AdviceAsync((Operation)operationData.Operation, (PickPartAdvice)(dynamic)advice);
            }

            if (result.Success)
                operationData.Advice(advice);

            return result;
        }
    }
}
