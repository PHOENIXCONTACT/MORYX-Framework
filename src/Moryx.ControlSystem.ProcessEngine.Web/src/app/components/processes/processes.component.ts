/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import {
  Component,
  computed,
  input,
  Input,
  OnDestroy,
  OnInit,
  signal,
} from "@angular/core";
import { MatListModule } from "@angular/material/list";
import {
  MatSlideToggleChange,
  MatSlideToggleModule,
} from "@angular/material/slide-toggle";
import {
  MoryxSnackbarService,
  NavigableEntryEditorComponent,
} from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { BehaviorSubject, Subscription } from "rxjs";
import { JobProcessModel } from "src/app/api/models/job-process-model";
import { ProcessActivityModel } from "src/app/api/models/process-activity-model";
import { ProcessEngineService } from "src/app/api/services";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { JobViewModel } from "src/app/models/job-view-model";
import { ProcessEngineStreamService } from "src/app/services/process-engine-stream.service";

@Component({
  selector: "app-processes",
  templateUrl: "./processes.component.html",
  imports: [
    CommonModule,
    TranslateModule,
    MatListModule,
    NavigableEntryEditorComponent,
    MatSlideToggleModule,
  ],
  providers: [
    ProcessEngineService,
    ProcessEngineStreamService,
    MoryxSnackbarService,
  ],
  styleUrls: ["./processes.component.scss"],
  standalone: true,
})
export class ProcessesComponent implements OnInit, OnDestroy {
  processes = signal<JobProcessModel[]>([]);
  processesAvailable = computed(() => this.processes().length > 0);
  selectedProcess = signal<JobProcessModel | undefined>(undefined);
  selectedActivity = signal<ProcessActivityModel | undefined>(undefined);
  possibleResources = computed(() => {
    const activity = this.selectedActivity();
    return activity?.possibleResources?.map((r) => r.name)?.join(", ");
  });
  showAll = signal(true);
  job = input.required<JobViewModel>();

  TranslationConstants = TranslationConstants;

  private processSubscription!: Subscription;
  private activitySubscription!: Subscription;

  constructor(
    public processEngineService: ProcessEngineService,
    public processEngineEvents: ProcessEngineStreamService,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService
  ) {
    console.log('loadede')
  }

  ngOnInit(): void {
    this.processEngineService
      .getRunningProcessesOfJob({
        jobId: this.job().model.id,
        allProcesses: this.showAll(),
      })
      .subscribe({
        next: (data) => {
          this.processes.update((_) => data);
          const firstProcess = this.processes().find(() => true);
          if (firstProcess) this.onSelectProcess(firstProcess);
        },
        error: async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.handleError(e),
      });

    this.processSubscription =
      this.processEngineEvents.updatedProcess.subscribe((p) =>
        this.onProcessUpdated(p)
      );

    this.activitySubscription =
      this.processEngineEvents.updatedActivity.subscribe((a) =>
        this.onActivityUpdated(a)
      );
  }

  ngOnDestroy(): void {
    this.processSubscription.unsubscribe();
    this.activitySubscription.unsubscribe();
  }

  onProcessUpdated(updatedProcess: JobProcessModel | undefined) {
    if (updatedProcess !== undefined) {
      // Extract job id
      // TODO: Extend model
      const jobId = BigInt(updatedProcess.id!) >> 14n;
      if (jobId !== BigInt(this.job().model.id!)) return;

      // Find old model and its index
      const oldProcess = this.processes().find(
        (p) => p.id === updatedProcess.id
      );
      if (oldProcess) {
        // Replace with new object from stream
        const index = this.processes().indexOf(oldProcess!);
        updatedProcess.activities = oldProcess.activities;
        this.processes.update(items => {
          items[index!] = updatedProcess;
          return items;
        });
      } else {
        // Add to activities of the process
        this.processes.update((items) => {
          items.push(updatedProcess);
          return items;
        });
      }
      if (this.selectedProcess()?.id === updatedProcess.id)
        this.selectedProcess.update((_) => updatedProcess);
    }
  }

  onActivityUpdated(updatedActivity: ProcessActivityModel | undefined) {
    if (!updatedActivity) return;
    // Extract process id
    // TODO: Replace with extended model
    const processId = BigInt(updatedActivity.id!) >> 14n;
    // Find this activities process
    const process = this.processes().find((p) => BigInt(p.id!) === processId);
    if (!process) return;

    // Find old model and its index
    const index = process.activities?.findIndex(
      (a) => a.id === updatedActivity.id
    );

    // Replace with new object from stream
    if (index! >= 0) process.activities![index!] = updatedActivity;
    // Or add to activities of the process
    else process.activities?.push(updatedActivity);

    if (this.selectedActivity()?.id === updatedActivity.id)
      this.selectedActivity.update((_) => updatedActivity);
  }

  onSelectProcess(process: JobProcessModel) {
    this.selectedProcess.update((_) => process);
    this.selectedActivity.update((_) => process.activities?.find(() => true));
  }

  onSelectActivity(activity: ProcessActivityModel) {
    this.selectedActivity.update((_) => activity);
  }

  setShowAll(change: MatSlideToggleChange) {
    this.showAll.update((_) => change.checked);
    this.processEngineService
      .getRunningProcessesOfJob({
        jobId: this.job().model.id,
        allProcesses: this.showAll(),
      })
      .subscribe({
        next: (data) => this.updateData(data),
        error: async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.handleError(e),
      });
  }

  private updateData(data: JobProcessModel[]) {
    this.processes.update((_) => data);
    this.selectedProcess.update(
      (_) =>
        data.find((p) => p.id === this.selectedProcess()?.id) ??
        data.find(() => true)
    );
    this.selectedActivity.update(
      (_) =>
        this.selectedProcess()?.activities?.find(
          (a) => a.id === this.selectedActivity()?.id
        ) ?? this.selectedProcess()?.activities?.find(() => true)
    );
  }
}

