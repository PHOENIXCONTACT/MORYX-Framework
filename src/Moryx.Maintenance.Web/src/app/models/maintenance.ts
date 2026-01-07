import { MaintenanceOrderDto } from "../api/models/Moryx/Maintenance/Endpoints/Dtos/maintenance-order-dto";
import { Acknowledgement, mapFromAcknowledgement } from "./acknowledgement";
import { Interval, IntervalBase, mapFromInterval } from "./interval-base";
import { MaintainableResource } from "./maintainable-resource";
import { mapFromVisualInstruction, VisualInstruction } from "./visual-instruction";

export interface Maintenance {
    id: number;
    resource: MaintainableResource;
    description?: string;
    interval: Interval;
    instructions: VisualInstruction[];
    block: boolean;
    isActive: boolean;
    created: Date;
    acknowledgements: Acknowledgement[];
    maintenanceStarted: boolean;
}

export function mapFrom(data: MaintenanceOrderDto): Maintenance{
    return <Maintenance>{
        id: data.id ?? 0,
        resource: data.resource,
        description: data.description,
        instructions: data.instructions?.map(x => mapFromVisualInstruction(x)),
        acknowledgements: data.acknowledgements?.map(e => mapFromAcknowledgement(e)),
        isActive: data.isActive,
        block: data.block,
        created: data.created ? new Date(data.created) : new Date(),
        interval: data.interval ? mapFromInterval(data.interval) : <Interval>{},
        maintenanceStarted: data.maintenanceStarted
    }
}