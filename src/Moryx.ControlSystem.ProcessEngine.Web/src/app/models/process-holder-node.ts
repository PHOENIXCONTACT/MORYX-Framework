/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ProcessHolderGroup } from "./process-holder-group-model";
import { ProcessHolderPosition } from "./process-holder-position-model";

export default interface ProcessHolderNode {
  name: string;
  data: ProcessHolderGroup | ProcessHolderPosition;
  children?: ProcessHolderNode[];
}
