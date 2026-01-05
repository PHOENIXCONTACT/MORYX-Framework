/* tslint:disable */
/* eslint-disable */
import { VisualInstruction as MoryxControlSystemVisualInstructionsVisualInstruction } from '../../../../../models/Moryx/ControlSystem/VisualInstructions/visual-instruction';
import { Acknowledgement as MoryxMaintenanceAcknowledgement } from '../../../../../models/Moryx/Maintenance/acknowledgement';
import { IntervalDto as MoryxMaintenanceEndpointsDtosIntervalDto } from '../../../../../models/Moryx/Maintenance/Endpoints/Dtos/interval-dto';
import { ResourceDto as MoryxMaintenanceEndpointsDtosResourceDto } from '../../../../../models/Moryx/Maintenance/Endpoints/Dtos/resource-dto';
export interface MaintenanceOrderDto {
  acknowledgements?: Array<MoryxMaintenanceAcknowledgement> | null;
  block?: boolean;
  created?: string;
  description?: string | null;
  id?: number;
  instructions?: Array<MoryxControlSystemVisualInstructionsVisualInstruction> | null;
  interval?: MoryxMaintenanceEndpointsDtosIntervalDto;
  isActive?: boolean;
  maintenanceStarted?: boolean;
  resource?: MoryxMaintenanceEndpointsDtosResourceDto;
}
