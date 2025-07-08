// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    public enum InternalOperationClassification
    {
        //
        // Summary:
        //     Classification during the creation
        Initial,
        //
        // Summary:
        //     Created operation and ready to start the production
        Ready,
        //
        // Summary:
        //     There is currently a working progress like the production or a reporting
        Running,
        //
        // Summary:
        //     The operation was interrupted but the production is currently running for the
        //     last parts
        Interrupting,
        //
        // Summary:
        //     The operation reached the current amount or the user has interrupted the operation
        Interrupted,
        //
        // Summary:
        //     The operation was declared as finished and can not be started again
        Completed,
        //
        // Summary:
        //     This operation was declared as aborted and was never started.
        Aborted,
        //
        // Summary:
        //     This operation is not finished, but has reached the targeted amount (not equal total amount of the order)
        AmountReached
    }
}

