/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { MethodEntry } from "../api/models";


export interface ResourceConstructionParameters {
  name: string;
  method: MethodEntry | undefined;
}

