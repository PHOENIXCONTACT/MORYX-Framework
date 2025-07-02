import { JobModel } from '../api/models';
export class JobViewModel {
  model: JobModel;

  productionSuccessPercent: number = 0;
  productionScrapPercent: number = 0;
  productionRunningPercent: number = 0;
  productionResidualPercent: number = 100;

  setupRunningPercent: number = 0;
  setupCompletedPercent: number = 0;
  setupResidualPercent: number = 100;

  constructor(model: JobModel) {
    this.model = model;
    this.updateProgress();
  }

  public updateModel(model: JobModel): void {
    this.model = model;
    this.updateProgress();
  }

  private updateProgress(): void {
    if (this.model.productionJob && this.model.productionJob.amount) {
      this.productionSuccessPercent = Math.round(
        (this.model.productionJob.successCount! * 100) /
          this.model.productionJob.amount
      );
      this.productionScrapPercent = Math.round(
        (this.model.productionJob.failureCount! * 100) /
          this.model.productionJob.amount
      );
      this.productionRunningPercent = Math.round(
        (this.model.productionJob.runningCount! * 100) /
          this.model.productionJob.amount
      );
      this.productionResidualPercent =
        100 -
        this.productionSuccessPercent -
        this.productionScrapPercent -
        this.productionRunningPercent;
    } else {
      this.productionSuccessPercent = 0;
      this.productionScrapPercent = 0;
      this.productionRunningPercent = 0;
      this.productionResidualPercent = 100;
    }

    if (this.model.setupJob && this.model.setupJob.steps) {
      let completed =
        this.model.setupJob.steps === undefined
          ? 0
          : this.model.setupJob.steps!.filter((s) => s.state === 'Completed')
              .length;
      let running =
        this.model.setupJob.steps === undefined
          ? 0
          : this.model.setupJob.steps!.filter((s) => s.state === 'Running')
              .length;
      let amount =
        this.model.setupJob.steps === undefined
          ? 0
          : this.model.setupJob.steps!.length;
      this.setupRunningPercent =
        amount > 0 ? Math.round((running * 100) / amount) : 0;
      this.setupCompletedPercent =
        amount > 0 ? Math.round((completed * 100) / amount) : 0;
      this.setupResidualPercent =
        100 - this.setupRunningPercent - this.setupCompletedPercent;
    } else {
      this.setupRunningPercent = 0;
      this.setupCompletedPercent = 0;
      this.setupResidualPercent = 1000;
    }
  }
}
