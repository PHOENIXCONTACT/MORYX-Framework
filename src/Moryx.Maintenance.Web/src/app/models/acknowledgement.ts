import { MoryxMaintenanceAcknowledgement } from "../api/models";

export interface Acknowledgement {
    id: number;
    operatorId: number;
    description?: string;
    created: Date;
}

export function mapFromAcknowledgement(data: MoryxMaintenanceAcknowledgement): Acknowledgement{
    return <Acknowledgement> {
        id: data.id,
        description: data.description,
        operatorId: data.operatorId,
        created: data.created ? new Date(data.created) : new Date()
    }
}