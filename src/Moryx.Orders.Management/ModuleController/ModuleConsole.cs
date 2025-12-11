// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Orders.Management
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public IOperationDataPool OperationDataPool { get; set; }

        public IOperationManager OperationManager { get; set; }

        [EntrySerialize]
        public string ImportTryAgain()
        {
            var operations = OperationDataPool.GetAll(o => o.State.Classification == OperationStateClassification.Initial)
                .ToArray();

            if (!operations.Any())
            {
                return "Noting to try again!";
            }

            operations.ForEach(delegate (IOperationData data)
            {
                data.Assign();
            });

            return "ok";
        }

        [EntrySerialize]
        public string AbortOperation(string orderNumber, string operationNumber)
        {
            var operation = OperationDataPool.Get(orderNumber, operationNumber).Result;
            if (operation == null)
            {
                return $"Operation {orderNumber}-{operationNumber} was not found!";
            }

            OperationManager.Abort(operation);

            return "ok";
        }
    }
}
