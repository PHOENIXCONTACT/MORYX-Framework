/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { ApiConfiguration } from 'src/app/api/api-configuration';
import { OperationModel } from '../api/models';
import { OperationAdvicedModel, OperationReportedModel, OperationStartedModel, OperationType } from 'src/app/models/operation-models';

@Injectable({
  providedIn: 'root',
})
export class OperationService {
  private apiConfiguration = inject(ApiConfiguration);
  private eventSource: EventSource;

  constructor() {
    this.eventSource = new EventSource(this.apiConfiguration.rootUrl + '/api/moryx/orders/stream');
  }

  public operationChanged(callback: (operationModel: OperationModel) => void) {
    // Register to progress
    this.eventSource.addEventListener(OperationType[OperationType.Progress], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (operationModel) {
        callback(operationModel!);
      }
    });
    // And updated
    this.eventSource.addEventListener(OperationType[OperationType.Update], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (operationModel) {
        callback(operationModel!);
      }
    });
  }

  // Deprecated: Only use as reference for operation types and payload
  public stream(operationType: OperationType, callbackFunction: Function) {

    this.eventSource.addEventListener(OperationType[OperationType.Start], event => {
      const operationStartedModel = JSON.parse(event.data) as OperationStartedModel;
      if (
        !operationStartedModel.operationModel ||
        !operationStartedModel.userId ||
        operationType !== OperationType.Start
      ) {
        return;
      }

      callbackFunction(operationStartedModel.operationModel!, operationStartedModel.userId!);
    });

    this.eventSource.addEventListener(OperationType[OperationType.Progress], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (!operationModel || operationType !== OperationType.Progress) {
        return;
      }

      callbackFunction(operationModel!);
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

      callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!);
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

      callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!);
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

      callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!);
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

      callbackFunction(operationadvicedModel.operationModel!, operationadvicedModel.adviceModel!);
    });

    this.eventSource.addEventListener(OperationType[OperationType.Update], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (!operationModel || operationType !== OperationType.Update) {
        return;
      }

      callbackFunction(operationModel!);
    });
  }
}

