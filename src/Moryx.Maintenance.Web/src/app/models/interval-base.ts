import { MoryxMaintenanceEndpointsDtosIntervalDto } from "../api/models";

export interface IntervalBase {
  lastMaintenanceDate: Date;
  elapsed: number;
  value: number;
  overdue: number;
  warning: number;
}

export interface Interval {
  interval: IntervalBase;
  type: IntervalType;
}

export enum IntervalType {
  Day = 0,
  Hour = 1,
  Cycle = 2,
}

export function mapFromInterval(data: MoryxMaintenanceEndpointsDtosIntervalDto): Interval{
  return <Interval>{
    interval: <IntervalBase>{
      value: <IntervalType>data.interval?.value ?? IntervalType.Day,
      elapsed: data.interval?.elapsed ?? 0,
      overdue: data.interval?.overdue ?? 0,
      lastMaintenanceDate: data.interval?.lastMaintenanceDate 
        ? new Date(data.interval?.lastMaintenanceDate)
        : new Date(),
      warning: data.interval?.warning ?? 0
    },
    type: data.type as any
  }
}