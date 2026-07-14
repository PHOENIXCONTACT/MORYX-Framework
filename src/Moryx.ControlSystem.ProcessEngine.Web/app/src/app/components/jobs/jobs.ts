/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from "@angular/common/http";
import { Component, effect, inject, OnInit, signal, untracked, ChangeDetectorRef, ChangeDetectionStrategy, DestroyRef } from "@angular/core";
import { JobManagementService, OrderManagementService } from "@api/services";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { JobViewModel } from "@app/models/job-view-model";
import { OperationType } from "@app/models/operation-models";
import { JobManagementStreamService } from "@app/services/job-management-stream.service";
import { OrderManagementStreamService } from "@app/services/order-management-stream.service";
import { environment } from "../../../environments/environment";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { EmptyState } from "@moryx/ngx-web-framework/empty-state";

import { MatExpansionModule } from "@angular/material/expansion";
import { MatIconModule } from "@angular/material/icon";
import { TranslatePipe } from "@ngx-translate/core";
import { Processes } from "../processes/processes";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatButtonModule } from "@angular/material/button";
import { JobModel } from "@api/models/job-model";
import { OperationModel } from "@api/models/operation-model";
import { ProcessEngineStreamService } from "@app/services/process-engine-stream.service";

@Component({
  selector: "app-jobs",
  templateUrl: "./jobs.html",
  styleUrls: ["./jobs.scss"],
  imports: [
    MatExpansionModule,
    MatIconModule,
    TranslatePipe,
    Processes,
    MatProgressSpinnerModule,
    EmptyState,
    MatProgressBarModule,
    MatButtonModule
  ],
  changeDetection: ChangeDetectionStrategy.Eager,
  providers: [],
  host: {
    '(window:beforeunload)': 'disconnectEvents()'
  }
})
export class Jobs implements OnInit {
  private destroyRef = inject(DestroyRef);
  private jobManagementService = inject(JobManagementService);
  private jobManagementEvents = inject(JobManagementStreamService);
  private orderManagementService = inject(OrderManagementService);
  private orderManagementEvents = inject(OrderManagementStreamService);
  private processEngineEvents = inject(ProcessEngineStreamService);
  private snackbarService = inject(SnackbarService);
  private changeDetectorRef = inject(ChangeDetectorRef);

  protected jobCollection = signal<JobViewModel[]>([]);
  protected operations = signal<OperationModel[]>([]);
  protected isLoading = signal<boolean>(true);

  protected environment = environment;
  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.destroyRef.onDestroy(() => this.disconnectEvents());

    effect(() => {
      const updatedJob = this.jobManagementEvents.updatedJob();
      untracked(() => {
        this.updateJobs(updatedJob);
      });
    });
  }

  ngOnInit(): void {
    this.fetchJobs();

    this.jobManagementEvents.connect();
    this.processEngineEvents.connect();

    this.orderManagementService.getOperations()
      .then((value) => this.operations.set(value))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));

    this.orderManagementEvents.connect(OperationType.Update, (updatedOperation: OperationModel) =>
        this.updateOperations(updatedOperation)
    );
  }

  private updateJobs(updatedJob: JobModel | undefined) {
    if (!updatedJob) {
      return;
    }

    const existingJob = this.jobCollection().find((j) => j.model.id === updatedJob.id);
    if (!existingJob) {
      this.jobCollection().push(new JobViewModel(updatedJob));
    } else if (updatedJob.state === "Completed") {
      this.jobCollection.update(current => current.filter((jVm) => jVm !== existingJob));
    } else {
      existingJob.updateModel(updatedJob);

      // TODO: This is a workaround to trigger change detection for the updated job.
      //  The JobViewModel is mutable and Angular does not detect changes to its properties.
      //  Consider refactoring JobViewModel to be immutable to avoid this issue.
      this.changeDetectorRef.markForCheck();
    }
  }

  private updateOperations(updatedOperation: OperationModel | undefined) {
    if (!updatedOperation) {
      return;
    }

    const existent = this.operations().find(
      (o) => o.identifier === updatedOperation.identifier
    );
    if (existent) {
      const index = this.operations().indexOf(existent);
      this.operations.update(items => {
        items[index] = updatedOperation;
        return items;
      });
    } else {
      this.operations.update(items => [...items, updatedOperation]);
    }
  }

  private fetchJobs() {
    this.jobManagementService.getAll().then((data) => {
      this.jobCollection.set(data.map((model) => new JobViewModel(model)));
      this.isLoading.set(false);
    }).catch(async (err: HttpErrorResponse) => {
      await this.snackbarService.handleError(err);
      this.isLoading.set(false);
    });
  }

  protected async onComplete(job: JobModel) {
    await this.jobManagementService
      .complete({jobId: job.id!})
      .catch((error) => this.snackbarService.handleError(error));
  }

  protected async onAbort(job: JobModel) {
    await this.jobManagementService
      .abort({jobId: job.id!})
      .catch((error) => this.snackbarService.handleError(error));
  }

  protected getOrderNumber(job: JobModel): string {
    if (job.productionJob) {
      return this.operations().find(operation => operation.jobIds?.find(j => j === job.id))?.order ?? "";
    }
    return "";
  }

  protected getOperationNumber(job: JobModel): string {
    if (job.productionJob) {
      return this.operations().find(operation => operation.jobIds?.find(j => j === job.id))?.number ?? "";
    }
    return "";
  }

  protected disconnectEvents() {
    this.jobManagementEvents.disconnect();
    this.processEngineEvents.disconnect();
    this.orderManagementEvents.disconnect();
  }
}

