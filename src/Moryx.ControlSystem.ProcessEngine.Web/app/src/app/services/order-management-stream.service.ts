/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { ApiConfiguration } from '@api/api-configuration';
import { OperationAdvicedModel, OperationReportedModel, OperationStartedModel, OperationType } from '@app/models/operation-models';
import { OperationModel } from '@api/models/operation-model';
import { ReportModel } from '@api/models/report-model';
import { AdviceModel } from '@api/models/advice-model';

interface OperationTypeCallbackMap {
  [OperationType.Start]: (operation: OperationModel, userId: string) => void;
  [OperationType.Progress]: (operation: OperationModel) => void;
  [OperationType.Update]: (operation: OperationModel) => void;
  [OperationType.Completed]: (operation: OperationModel, report: ReportModel) => void;
  [OperationType.Interrupted]: (operation: OperationModel, report: ReportModel) => void;
  [OperationType.Report]: (operation: OperationModel, report: ReportModel) => void;
  [OperationType.Advice]: (operation: OperationModel, advice: AdviceModel) => void;
}

@Injectable({
  providedIn: 'root',
})
export class OrderManagementStreamService {
  private config = inject(ApiConfiguration);

  private eventSource?: EventSource;

  public connect<T extends OperationType>(operationType: T, callbackFunction: OperationTypeCallbackMap[T]) {
    this.eventSource = new EventSource(this.config.rootUrl + '/api/moryx/orders/stream');
    // Cast needed because TypeScript cannot narrow the generic T inside the method body
    const callback = callbackFunction as (...args: unknown[]) => void;

    this.eventSource.addEventListener(OperationType[OperationType.Start], event => {
      const operationStartedModel = JSON.parse(event.data) as OperationStartedModel;
      if (
        !operationStartedModel.operationModel ||
        !operationStartedModel.userId ||
        operationType !== OperationType.Start
      ) {
        return;
      }

      callback(operationStartedModel.operationModel!, operationStartedModel.userId!);
    });

    this.eventSource.addEventListener(OperationType[OperationType.Progress], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (!operationModel || operationType !== OperationType.Progress) {
        return;
      }

      callback(operationModel!);
    });

    this.eventSource.addEventListener(OperationType[OperationType.Completed], event => {
      const operationReportedModel = JSON.parse(event.data) as OperationReportedModel;
      if (
        !operationReportedModel.operationModel ||
        !operationReportedModel.reportModel ||
        operationType !== OperationType.Completed
      ) {
        return;
      }

      callback(operationReportedModel.operationModel!, operationReportedModel.reportModel!);
    });

    this.eventSource.addEventListener(OperationType[OperationType.Interrupted], event => {
      const operationReportedModel = JSON.parse(event.data) as OperationReportedModel;
      if (
        !operationReportedModel.operationModel ||
        !operationReportedModel.reportModel ||
        operationType !== OperationType.Interrupted
      ) {
        return;
      }

      callback(operationReportedModel.operationModel!, operationReportedModel.reportModel!);
    });

    this.eventSource.addEventListener(OperationType[OperationType.Report], event => {
      const operationReportedModel = JSON.parse(event.data) as OperationReportedModel;
      if (
        !operationReportedModel.operationModel ||
        !operationReportedModel.reportModel ||
        operationType !== OperationType.Report
      ) {
        return;
      }

      callback(operationReportedModel.operationModel!, operationReportedModel.reportModel!);
    });

    this.eventSource.addEventListener(OperationType[OperationType.Advice], event => {
      const operationadvicedModel = JSON.parse(event.data) as OperationAdvicedModel;
      if (
        !operationadvicedModel.operationModel ||
        !operationadvicedModel.adviceModel ||
        operationType !== OperationType.Advice
      ) {
        return;
      }

      callback(operationadvicedModel.operationModel!, operationadvicedModel.adviceModel!);
    });

    this.eventSource.addEventListener(OperationType[OperationType.Update], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (!operationModel || operationType !== OperationType.Update) {
        return;
      }

      callback(operationModel!);
    });
  }

  disconnect() {
    if (this.eventSource) {
      this.eventSource.close();
      this.eventSource = undefined;
    }
  }
}

