/* tslint:disable */
/* eslint-disable */
import { IntervalType as MoryxMaintenanceEndpointsDtosIntervalType } from '../../../../../models/Moryx/Maintenance/Endpoints/Dtos/interval-type';
import { IntervalBase as MoryxMaintenanceIntervalTypesIntervalBase } from '../../../../../models/Moryx/Maintenance/IntervalTypes/interval-base';
export interface IntervalDto {
  interval?: MoryxMaintenanceIntervalTypesIntervalBase;
  type?: MoryxMaintenanceEndpointsDtosIntervalType;
}
