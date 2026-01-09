import { ProcessHolderGroup } from "./process-holder-group-model";
import { ProcessHolderPosition } from "./process-holder-position-model";

export default interface ProcessHolderNode {
  name: string;
  data: ProcessHolderGroup | ProcessHolderPosition;
  children?: ProcessHolderNode[];
}