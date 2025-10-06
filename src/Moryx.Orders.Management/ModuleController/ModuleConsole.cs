// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Orders.Management
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public IOperationDataPool OperationDataPool { get; set; }

        public IOperationManager OperationManager { get; set; }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            if (!args.Any())
                outputStream("OrderManagement console requires arguments");

            switch (args[0])
            {
                case "import":
                    Import(args.Skip(1).ToArray(), outputStream);
                    break;
                case "operation":
                    Operation(args.Skip(1).ToArray(), outputStream);
                    break;
            }
        }

        private void Import(string[] args, Action<string> outputStream)
        {
            if (!args.Any())
                outputStream("Import needs additional arguments e.g.: try");

            switch (args[0])
            {
                case "try":
                    ImportTryAgain(outputStream);
                    break;
            }
        }

        private void Operation(string[] args, Action<string> outputStream)
        {
            if (args.Length == 0)
                return;
            switch (args[0])
            {
                case "abort":
                    AbortOperation(args.Skip(1).ToArray(), outputStream);
                    break;
            }
        }

        private void ImportTryAgain(Action<string> outputStream)
        {
            var operations = OperationDataPool.GetAll(o => o.State.Classification == OperationClassification.Initial)
                .ToArray();

            if (!operations.Any())
            {
                outputStream("Noting to try again!");
                return;
            }

            outputStream($"Trying to import {operations.Length} operations ...");
            operations.ForEach(delegate (IOperationData data)
            {
                outputStream($"{data.OrderData.Number}-{data.Number}");
                data.Assign();
            });
        }

        private void AbortOperation(string[] toArray, Action<string> outputStream)
        {
            if (toArray.Length != 2)
                return;

            var operation = OperationDataPool.Get(toArray[0], toArray[1]);
            if (operation == null)
            {
                outputStream($"Operation {toArray[0]}-{toArray[1]} was not found!");
                return;
            }

            OperationManager.Abort(operation);
        }
    }
}

