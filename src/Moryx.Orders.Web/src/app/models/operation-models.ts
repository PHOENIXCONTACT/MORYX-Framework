import { AdviceModel } from "../api/models/Moryx/Orders/Endpoints/advice-model";
import { OperationModel } from "../api/models/Moryx/Orders/Endpoints/operation-model";
import { ReportModel } from "../api/models/Moryx/Orders/Endpoints/report-model";


export class OperationChangedModelBase {
  operationModel: OperationModel | undefined;
}

export class OperationStartedModel implements OperationChangedModelBase {
  operationModel: OperationModel | undefined;
  userId: string | undefined;
}

export class OperationReportedModel implements OperationChangedModelBase {
  operationModel: OperationModel | undefined;
  reportModel: ReportModel | undefined;
}

export class OperationAdvicedModel implements OperationChangedModelBase {
  operationModel: OperationModel | undefined;
  adviceModel: AdviceModel | undefined;
}

export enum OperationType {
  Start,
  Progress,
  Completed,
  Interrupted,
  Report,
  Advice,
  Update,
}
