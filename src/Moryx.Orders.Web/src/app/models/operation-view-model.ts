import { OperationModel } from '../api/models';
import { OperationStateClassification } from '../api/models';

export class OperationViewModel{
    model: OperationModel;
    errorMessagesCount: number = 0;

    progressSuccessPercent: number = 0;
    progressScrapPercent: number = 0;
    progressRunningPercent: number = 0;
    progressPendingPercent: number = 0;
    progressResidualPercent: number = 100;

    overDelivery: number = 0;
    underDelivery: number = 0;

    constructor(model: OperationModel)
    {
        this.model = model;
        this.updateProgress();
        this.updateDeliveryThreshold();
    }

    public updateModel(model: OperationModel): void
    {
        this.model = model;
        this.updateProgress();
        this.updateDeliveryThreshold();
    }

    private updateProgress(): void {
        if(this.model.totalAmount) {
            if(this.model.progressSuccess && this.model.progressSuccess > 0)
                this.progressSuccessPercent = Math.round(this.model.progressSuccess*100/this.model.totalAmount);
            else
                this.progressSuccessPercent = 0;

            if(this.model.progressScrap && this.model.progressScrap > 0)
                this.progressScrapPercent = Math.round(this.model.progressScrap*100/this.model.totalAmount);
            else
                this.progressScrapPercent = 0;

            if(this.model.progressRunning && this.model.progressRunning > 0)
                this.progressRunningPercent = Math.round(this.model.progressRunning*100/this.model.totalAmount);
            else
                this.progressRunningPercent = 0;

            if(this.model.progressPending && this.model.progressPending > 0)
                this.progressPendingPercent = Math.round(this.model.progressPending*100/this.model.totalAmount);
            else
                this.progressPendingPercent = 0;

            this.progressResidualPercent = 100 - this.progressSuccessPercent - this.progressScrapPercent - this.progressRunningPercent - this.progressPendingPercent;

            if(this.progressResidualPercent < 0)
                this.progressResidualPercent = 0;
        }
    }

    private updateDeliveryThreshold(): void {
        if(this.model.totalAmount) {
            if(this.model.overDeliveryAmount)
                this.overDelivery = Math.round(this.model.overDeliveryAmount / this.model.totalAmount) * 100;
            
            if(this.model.underDeliveryAmount)
                this.underDelivery = Math.round(this.model.underDeliveryAmount / this.model.totalAmount) * 100;
        }
    }

    get isProducing() {
        if(this.model.classification === OperationStateClassification.Running) {
            if(this.model.progressRunning && this.model.progressRunning > 0) {
                return true;
            }
            else if(this.model.progressPending && this.model.progressPending > 0) {
                return true;
            }
        }

        return false;
    }
}
