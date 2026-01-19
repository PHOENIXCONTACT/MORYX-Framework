/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { InternalOperationClassification } from '../api/models/internal-operation-classification';

// flat order Model used in the entire UI
export default interface Order {
    isToggled: boolean,
    orderNumber: string;
    operationNumber: string;
    orderColor: string;
    classification?: InternalOperationClassification;
}
