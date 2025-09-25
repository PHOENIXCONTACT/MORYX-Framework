import { HttpErrorResponse } from "@angular/common/http";
import { Component, importProvidersFrom, OnInit, signal } from "@angular/core";
import {
  JobManagementService,
  OrderManagementService,
} from "src/app/api/services";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { JobViewModel } from "src/app/models/job-view-model";
import { OperationType } from "src/app/models/operation-models";
import { JobManagementStreamService } from "src/app/services/job-management-stream.service";
import { OrderManagementStreamService } from "src/app/services/order-management-stream.service";
import { environment } from "src/environments/environment";
import "../../extensions/observable.extensions";
import "./../../extensions/observable.extensions";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import {
  EmptyStateComponent,
  MoryxSnackbarService,
} from "@moryx/ngx-web-framework";
import { CommonModule } from "@angular/common";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatIconModule } from "@angular/material/icon";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { ProcessesComponent } from "../processes/processes.component";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatButtonModule } from "@angular/material/button";
import { JobModel } from "src/app/api/models/Moryx/ControlSystem/Jobs/Endpoints/job-model";
import { OperationModel } from "src/app/api/models/Moryx/Orders/Endpoints/operation-model";

@Component({
  selector: "app-jobs",
  templateUrl: "./jobs.component.html",
  styleUrls: ["./jobs.component.scss"],
  imports: [
    CommonModule,
    MatExpansionModule,
    MatIconModule,
    TranslateModule,
    ProcessesComponent,
    MatProgressSpinnerModule,
    EmptyStateComponent,
    MatProgressBarModule,
    MatButtonModule
  ],
  providers: [],
  standalone: true,
})
export class JobsComponent implements OnInit {
  jobCollection = signal<JobViewModel[]>([]);
  operations =signal<OperationModel[]>([]);
  isLoading = signal<boolean>(true);

  environment = environment;
  TranslationConstants = TranslationConstants;

  constructor(
    public jobManagementService: JobManagementService,
    public jobManagementEvents: JobManagementStreamService,
    public orderManagementService: OrderManagementService,
    public translate: TranslateService,
    private orderManagementEvents: OrderManagementStreamService,
    private moryxSnackbar: MoryxSnackbarService
  ) {}

  ngOnInit(): void {
    this.fetchJobs();

    this.jobManagementEvents.updatedJob.subscribe((updatedJob) =>
      this.updateJobs(updatedJob)
    );

    this.orderManagementService.getOperations().subscribe({
      next: (value) => this.operations.update(_=> value),
      error: async (e: HttpErrorResponse) =>
        await this.moryxSnackbar.handleError(e),
    });

    this.orderManagementEvents.stream(
      OperationType.Update,
      (updatedOperation: OperationModel) =>
        this.updateOperations(updatedOperation)
    );
  }

  private updateJobs(updatedJob: JobModel | undefined) {
    if (!updatedJob) return;

    const jobVM = this.jobCollection().find((j) => j.model.id === updatedJob.id);
    if (!jobVM) this.jobCollection().push(new JobViewModel(updatedJob));
    else if (updatedJob.state === "Completed")
      this.jobCollection.update(_=> this.jobCollection().filter((jVm) => jVm !== jobVM));
    else jobVM.updateModel(updatedJob);
  }

  private updateOperations(updatedOperation: OperationModel | undefined) {
    if (!updatedOperation) return;

    var existent = this.operations().find(
      (o) => o.identifier === updatedOperation.identifier
    );
    if (existent) {
      let index = this.operations().indexOf(existent);
      this.operations.update(items => {
        items[index] = updatedOperation;
        return items;
      });
    } else {
      this.operations.update(items => [...items, updatedOperation]);
    }
  }

  private fetchJobs() {
    this.jobManagementService.getAll_1().subscribe({
      next: (data) => {
        this.jobCollection.update(_=> data.map((model) => new JobViewModel(model)));
        this.isLoading.update(_=> false);
      },
      error: async (err: HttpErrorResponse) => {
        await this.moryxSnackbar.handleError(err);
        this.isLoading.update(_=> false);
      },
    });
  }

  async onComplete(job: JobModel) {
    await this.jobManagementService
      .complete({ jobId: job.id! })
      .toAsync()
      .catch(async (error) => await this.moryxSnackbar.handleError(error));
  }

  async onAbort(job: JobModel) {
    await this.jobManagementService
      .abort({ jobId: job.id! })
      .toAsync()
      .catch(async (error) => await this.moryxSnackbar.handleError(error));
  }

  getOrderNumber(job: JobModel): string {
    if (job.productionJob) 
      return this.operations().find(operation => operation.jobIds?.find(j => j === job.id))?.order!;
    return "";
  }

  getOperationNumber(job: JobModel): string {
    if (job.productionJob)
      return this.operations().find(operation => operation.jobIds?.find(j => j === job.id))?.number!;
    return "";
  }
}
