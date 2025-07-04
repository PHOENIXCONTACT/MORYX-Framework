﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Orders.Management.Assignment
{
    [PluginFactory(typeof(INameBasedComponentSelector))]
    internal interface IAssignStepFactory
    {
        IOperationAssignStep Create(string name);
    }
}
