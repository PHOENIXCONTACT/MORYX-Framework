/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { OperationModel } from '../api/models';
import { OperationStateClassification } from '../api/models';

export class OperationViewModel {
  model: OperationModel;
  errorMessagesCount: number = 0;

  overDelivery: number = 0;
  underDelivery: number = 0;

  constructor(model: OperationModel) {
    this.model = model;
    this.updateDeliveryThreshold();
  }

  public updateModel(model: OperationModel): void {
    this.model = model;
    this.updateDeliveryThreshold();
  }

  private updateDeliveryThreshold(): void {
    if (this.model.totalAmount) {
      if (this.model.overDeliveryAmount)
        this.overDelivery = Math.round(this.model.overDeliveryAmount / this.model.totalAmount) * 100;

      if (this.model.underDeliveryAmount)
        this.underDelivery = Math.round(this.model.underDeliveryAmount / this.model.totalAmount) * 100;
    }
  }

  get isProducing() {
    if (this.model.classification === OperationStateClassification.Running) {
      if (this.model.progressRunning && this.model.progressRunning > 0) {
        return true;
      } else if (this.model.progressPending && this.model.progressPending > 0) {
        return true;
      }
    }
    return false;
  }
}
