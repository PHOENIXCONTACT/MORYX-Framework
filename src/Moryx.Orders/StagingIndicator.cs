// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders
{
    /// <summary>
    /// The material staging indicator defines the method of how needed materials can be supplied for production with the assistance of the Warehouse Management system.
    /// As with the location coordinates(warehouse number, storage type, and storage bin), the system files this indicator in the control cycle.
    /// </summary>
    public enum StagingIndicator
    {
        // Source: https://www.consolut.com/s/sap-ides-zugriff/d/e/doc/G-BERKZ/

        /// <summary>
        /// These materials are not relevant to WM staging. They cannot be requested using the WM system.
        /// </summary>
        NotRelevant = 0,

        /// <summary>
        /// These materials are picked according to the required quantity specified in the production order.
        /// </summary>
        PickPart = 1,

        /// <summary>
        /// These materials are always removed from the warehouse in full cases.
        /// These can be ordered, for example, as soon as a case of needed parts is emptied in production.
        /// </summary>
        CrateOrKanbanPart = 2,

        /// <summary>
        /// Release order parts are scheduled individually and the quantities are supplied manually to replenishment
        /// storage bins based on the requirements of production orders and the stock levels in the scheduled production supply areas.
        /// </summary>
        ReleaseOrderPart = 3,

        /// <summary>
        /// Materials are staged manually. For example, you can transport individual components using manually created transfer orders,
        /// or you can use the bypass method to transport them directly from the goods receipt zone to production.
        /// In goods receipt posting, postings are made to the production storage bins from the control cycles.
        /// </summary>
        ManualStaging = 4,

        /// <summary>
        /// These materials are staged directly in WM using a production material request
        /// </summary>
        EwmStaging = 5
    }
}