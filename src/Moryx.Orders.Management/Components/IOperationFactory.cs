// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Orders.Management
{
    [PluginFactory]
    internal interface IOperationFactory
    {
        IOperationData Create(IOperationSavingContext savingContext);

        void Destroy(IOperationData operationData);
    }
}
