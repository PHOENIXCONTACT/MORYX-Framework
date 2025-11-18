import { VisualizationModel } from "../api/models/visualization-model";
import { ProcessHolderPosition } from "./process-holder-position-model";

export interface ProcessHolderGroup {
    id: number;
    name: string;
    isEmpty: boolean;
    visualization?: VisualizationModel;
    positions: Array<ProcessHolderPosition>
}