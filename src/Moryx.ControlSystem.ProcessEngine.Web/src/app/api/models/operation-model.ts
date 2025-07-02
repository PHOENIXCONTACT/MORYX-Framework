/* tslint:disable */
/* eslint-disable */
import { OperationClassification } from './operation-classification';
import { SetupClassification } from './setup-classification';
export interface OperationModel {
  canAdvice?: boolean;
  canAssign?: boolean;
  canBegin?: boolean;
  canInterrupt?: boolean;
  canReport?: boolean;
  classification?: OperationClassification;
  end?: null | string;
  hasDocuments?: boolean;
  hasPartList?: boolean;
  identifier?: string;
  isAborted?: boolean;
  isAmountReached?: boolean;
  isAssigning?: boolean;
  isCreated?: boolean;
  isFailed?: boolean;
  jobIds?: null | Array<number>;
  name?: null | string;
  number?: null | string;
  order?: null | string;
  overDeliveryAmount?: number;
  pendingCount?: number;
  plannedEnd?: string;
  plannedStart?: string;
  productId?: number;
  productIdentifier?: null | string;
  productName?: null | string;
  productRevision?: number;
  progressPending?: number;
  progressRunning?: number;
  progressScrap?: number;
  progressSuccess?: number;
  recipeIds?: null | Array<number>;
  recipeName?: null | string;
  reportedFailureCount?: number;
  reportedSuccessCount?: number;
  runningCount?: number;
  scrapCount?: number;
  /** @deprecated */setupRequirement?: null | {
[key: string]: SetupClassification;
};
  sortOrder?: number;
  start?: null | string;
  /** @deprecated */state?: null | string;
  stateDisplayName?: null | string;
  successCount?: number;
  targetCycleTime?: number;
  totalAmount?: number;
  underDeliveryAmount?: number;
  unit?: null | string;
}
