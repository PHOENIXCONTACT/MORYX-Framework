/* tslint:disable */
/* eslint-disable */
import { ICapabilities as MoryxAbstractionLayerCapabilitiesICapabilities } from '../../../models/Moryx/AbstractionLayer/Capabilities/i-capabilities';
export interface IOperatorAssignable {
  capabilities?: MoryxAbstractionLayerCapabilitiesICapabilities;
  id?: number;
  name?: string | null;
  requiredSkills?: Array<MoryxAbstractionLayerCapabilitiesICapabilities> | null;
}
