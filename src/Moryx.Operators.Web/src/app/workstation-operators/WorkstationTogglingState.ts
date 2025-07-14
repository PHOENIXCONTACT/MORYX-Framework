import { WorkstationViewModel } from '../models/workstation-view-model';

//workstation toggling state model
export interface WorkstationTogglingState {
  station: WorkstationViewModel;
  isExpanded: boolean;
}
