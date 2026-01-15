/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Observable } from "rxjs";
import { OperationViewModel } from "src/app/models/operation-view-model";


export interface InterruptDialogData {
  operation: OperationViewModel;
  onSubmit: (guid: string, user: string | undefined) => Observable<void>;
}

