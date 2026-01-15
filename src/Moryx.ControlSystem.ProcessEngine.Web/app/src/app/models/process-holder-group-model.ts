/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { VisualizationModel } from "../api/models/visualization-model";
import { ProcessHolderPosition } from "./process-holder-position-model";

export interface ProcessHolderGroup {
    id: number;
    name: string;
    isEmpty: boolean;
    visualization?: VisualizationModel;
    positions: Array<ProcessHolderPosition>
}
