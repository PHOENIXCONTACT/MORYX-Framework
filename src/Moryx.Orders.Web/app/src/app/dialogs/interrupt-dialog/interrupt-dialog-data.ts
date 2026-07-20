/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { OperationViewModel } from "@app/models/operation-view-model";

export interface InterruptDialogData {
  operation: OperationViewModel;
  onSubmit: (guid: string, user: string | undefined) => Promise<void>;
}

