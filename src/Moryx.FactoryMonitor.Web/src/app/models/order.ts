import { InternalOperationClassification } from '../api/models/Moryx/FactoryMonitor/Endpoints/Models/internal-operation-classification';

// flat order Model used in the entire UI
export default interface Order {
    isToggled: boolean,
    orderNumber: string;
    operationNumber: string;
    orderColor: string;
    classification?: InternalOperationClassification;
}