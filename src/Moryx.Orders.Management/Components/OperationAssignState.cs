// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management
{
    [Flags]
    internal enum OperationAssignState
    {
        Initial = 1 << 1,

        Assigned = 1 << 2,

        Changed = 1 << 3,

        Failed = Changed | 1 << 4,
    }
}
