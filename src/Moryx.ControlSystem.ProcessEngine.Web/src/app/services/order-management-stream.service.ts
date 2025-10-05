import { Injectable, NgZone } from '@angular/core';
import { ApiConfiguration } from 'src/app/api/api-configuration';
import { OperationAdvicedModel, OperationReportedModel, OperationStartedModel, OperationType } from 'src/app/models/operation-models';
import { OperationModel } from '../api/models/Moryx/Orders/Endpoints/operation-model';

@Injectable({
  providedIn: 'root',
})
export class OrderManagementStreamService {
  constructor(private config: ApiConfiguration, private zone: NgZone) {}

  public stream(operationType: OperationType, callbackFunction: Function) {
    const eventSource = new EventSource(this.config.rootUrl + '/api/moryx/orders/stream');

    eventSource.addEventListener(OperationType[OperationType.Start], event => {
      const operationStartedModel = JSON.parse(event.data) as OperationStartedModel;
      if (
        !operationStartedModel.operationModel ||
        !operationStartedModel.userId ||
        operationType !== OperationType.Start
      ) {
        return;
      }

      this.zone.run(() => callbackFunction(operationStartedModel.operationModel!, operationStartedModel.userId!));
    });

    eventSource.addEventListener(OperationType[OperationType.Progress], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (!operationModel || operationType !== OperationType.Progress) {
        return;
      }

      this.zone.run(() => callbackFunction(operationModel!));
    });

    eventSource.addEventListener(OperationType[OperationType.Completed], event => {
      const operationReportedModel = JSON.parse(event.data) as OperationReportedModel;
      if (
        !operationReportedModel.operationModel ||
        !operationReportedModel.reportModel ||
        operationType !== OperationType.Completed
      ) {
        return;
      }

      this.zone.run(() => callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!));
    });

    eventSource.addEventListener(OperationType[OperationType.Interrupted], event => {
      const operationReportedModel = JSON.parse(event.data) as OperationReportedModel;
      if (
        !operationReportedModel.operationModel ||
        !operationReportedModel.reportModel ||
        operationType !== OperationType.Interrupted
      ) {
        return;
      }

      this.zone.run(() => callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!));
    });

    eventSource.addEventListener(OperationType[OperationType.Report], event => {
      const operationReportedModel = JSON.parse(event.data) as OperationReportedModel;
      if (
        !operationReportedModel.operationModel ||
        !operationReportedModel.reportModel ||
        operationType !== OperationType.Report
      ) {
        return;
      }

      this.zone.run(() => callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!));
    });

    eventSource.addEventListener(OperationType[OperationType.Advice], event => {
      const operationadvicedModel = JSON.parse(event.data) as OperationAdvicedModel;
      if (
        !operationadvicedModel.operationModel ||
        !operationadvicedModel.adviceModel ||
        operationType !== OperationType.Advice
      ) {
        return;
      }

      this.zone.run(() => callbackFunction(operationadvicedModel.operationModel!, operationadvicedModel.adviceModel!));
    });

    eventSource.addEventListener(OperationType[OperationType.Update], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (!operationModel || operationType !== OperationType.Update) {
        return;
      }
      
      this.zone.run(() => callbackFunction(operationModel!));
    });
  }
}
