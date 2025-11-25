import { MethodEntry } from "../api/models";


export interface ResourceConstructionParameters {
  name: string;
  method: MethodEntry | undefined;
}
