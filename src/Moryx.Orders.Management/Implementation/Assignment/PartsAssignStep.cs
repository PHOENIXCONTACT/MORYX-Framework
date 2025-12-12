// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationAssignStep), Name = nameof(PartsAssignStep))]
    internal class PartsAssignStep : IOperationAssignStep
    {
        public IPartsAssignment PartsAssignment { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public async Task<bool> AssignStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            IEnumerable<ProductPart> parts = null;
            try
            {
                parts = await PartsAssignment.LoadPartsAsync(operationData.Operation, operationLogger, CancellationToken.None);
            }
            catch (Exception e)
            {
                operationLogger.LogException(LogLevel.Error, e, Strings.PartsAssignStep_Threw_Exception, PartsAssignment.GetType());
            }

            if (parts is null)
            {
                operationLogger.Log(LogLevel.Error, Strings.PartsAssignStep_Failed);
                return false;
            }

            operationData.Operation.Parts = parts.ToList();
            return true;
        }

        public Task<bool> RestoreStep(IOperationData operationData, IOperationLogger operationLogger)
            => Task.FromResult(true);
    }
}

