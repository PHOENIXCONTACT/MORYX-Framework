import { Injectable, NgZone } from '@angular/core';
import { ApiConfiguration } from 'src/app/api/api-configuration';
import { OperationModel } from 'src/app/api/models/Moryx/Orders/Endpoints/operation-model';
import { OperationAdvicedModel, OperationReportedModel, OperationStartedModel, OperationType } from 'src/app/models/operation-models';

@Injectable({
  providedIn: 'root',
})
export class OperationService {
  
  private eventSource: EventSource;

  constructor(private config: ApiConfiguration, private zone: NgZone) {
    this.eventSource = new EventSource(this.config.rootUrl + '/api/moryx/orders/stream');
  }


  public operationChanged(callback: (operationModel: OperationModel) => void) {
    // Register to progress
    this.eventSource.addEventListener(OperationType[OperationType.Progress], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (operationModel) {
        this.zone.run(() => callback(operationModel!));
      }      
    });
    // And updated
    this.eventSource.addEventListener(OperationType[OperationType.Update], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (operationModel) {
        this.zone.run(() => callback(operationModel!));
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

      this.zone.run(() => callbackFunction(operationStartedModel.operationModel!, operationStartedModel.userId!));
    });

    this.eventSource.addEventListener(OperationType[OperationType.Progress], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (!operationModel || operationType !== OperationType.Progress) {
        return;
      }

      this.zone.run(() => callbackFunction(operationModel!));
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

      this.zone.run(() => callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!));
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

      this.zone.run(() => callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!));
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

      this.zone.run(() => callbackFunction(operationReportedModel.operationModel!, operationReportedModel.reportModel!));
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

      this.zone.run(() => callbackFunction(operationadvicedModel.operationModel!, operationadvicedModel.adviceModel!));
    });

    this.eventSource.addEventListener(OperationType[OperationType.Update], event => {
      const operationModel = JSON.parse(event.data) as OperationModel;
      if (!operationModel || operationType !== OperationType.Update) {
        return;
      }
      
      this.zone.run(() => callbackFunction(operationModel!));
    });
  }
}
