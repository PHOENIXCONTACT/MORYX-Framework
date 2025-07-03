using System;

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