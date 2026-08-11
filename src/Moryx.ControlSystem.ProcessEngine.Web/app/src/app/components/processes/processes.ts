/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { DatePipe } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import {
  ChangeDetectorRef,
  Component,
  computed,
  effect,
  inject,
  input,
  OnInit,
  signal,
  untracked,
  ChangeDetectionStrategy
} from "@angular/core";
import { MatListModule } from "@angular/material/list";
import { MatSlideToggleChange, MatSlideToggleModule } from "@angular/material/slide-toggle";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { NavigableEntryEditor } from "@moryx/ngx-web-framework/entry-editor";
import { TranslatePipe } from "@ngx-translate/core";
import { JobProcessModel } from "@api/models/job-process-model";
import { ProcessActivityModel } from "@api/models/process-activity-model";
import { ProcessEngineService } from "@api/services";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { JobViewModel } from "@app/models/job-view-model";
import { ProcessEngineStreamService } from "@app/services/process-engine-stream.service";

@Component({
  selector: "app-processes",
  templateUrl: "./processes.html",
  imports: [
    DatePipe,
    TranslatePipe,
    MatListModule,
    NavigableEntryEditor,
    MatSlideToggleModule,
  ],
  providers: [
    ProcessEngineService,
    ProcessEngineStreamService,
    SnackbarService,
  ],
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ["./processes.scss"]
})
export class Processes implements OnInit {
  private processEngineService = inject(ProcessEngineService);
  private processEngineEvents = inject(ProcessEngineStreamService);
  private snackbarService = inject(SnackbarService);
  private changeDetectorRef = inject(ChangeDetectorRef);

  protected processes = signal<JobProcessModel[]>([]);
  protected processesAvailable = computed(() => this.processes().length > 0);
  protected selectedProcess = signal<JobProcessModel | undefined>(undefined);
  protected selectedActivity = signal<ProcessActivityModel | undefined>(undefined);
  protected possibleResources = computed(() => {
    const activity = this.selectedActivity();
    return activity?.possibleResources?.map((r) => r.name)?.join(", ");
  });
  protected showAll = signal(true);
  readonly job = input.required<JobViewModel>();

  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const process = this.processEngineEvents.updatedProcess();
      untracked(() => {
        this.onProcessUpdated(process);
      });
    });
    effect(() => {
      const activity = this.processEngineEvents.updatedActivity();
      untracked(() => {
        this.onActivityUpdated(activity);
      });
    });
  }

  ngOnInit(): void {
    this.processEngineService
      .getRunningProcessesOfJob({
        jobId: this.job().model.id,
        allProcesses: this.showAll()
      })
      .then((data) => {
        this.processes.update((_) => data);
        const firstProcess = this.processes().find(() => true);
        if (firstProcess) {
          this.onSelectProcess(firstProcess);
        }
      })
      .catch(async (e: HttpErrorResponse) =>
        await this.snackbarService.handleError(e));
  }

  private onProcessUpdated(updatedProcess: JobProcessModel | undefined) {
    if (updatedProcess !== undefined) {
      // Extract job id
      // TODO: Extend model
      const jobId = BigInt(updatedProcess.id!) >> 14n;
      if (jobId !== BigInt(this.job().model.id!)) {
        return;
      }

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
      if (this.selectedProcess()?.id === updatedProcess.id) {
        this.selectedProcess.update((_) => updatedProcess);
      }
    }
  }

  private onActivityUpdated(updatedActivity: ProcessActivityModel | undefined) {
    if (!updatedActivity) {
      return;
    }

    // Extract process id
    // TODO: Replace with extended model
    const processId = BigInt(updatedActivity.id!) >> 14n;
    // Find this activities process
    const process = this.processes().find((p) => BigInt(p.id!) === processId);
    if (!process) {
      return;
    }

    // Find old model and its index
    const index = process.activities?.findIndex(
      (a) => a.id === updatedActivity.id
    );

    if (index! >= 0) {
      // Replace with new object from stream
      process.activities![index!] = updatedActivity;

    } else {
      // Or add to activities of the process
      process.activities?.push(updatedActivity);
    }

    // TODO: This is a workaround to trigger change detection for the activity list,
    //  as the process object reference does not change with the updated activity.
    //  This should be fixed by using immutable data structures or by extending the model with a method to update activities.
    this.changeDetectorRef.markForCheck();

    if (this.selectedActivity()?.id === updatedActivity.id) {
      this.selectedActivity.update((_) => updatedActivity);
    }
  }

  protected onSelectProcess(process: JobProcessModel) {
    this.selectedProcess.update((_) => process);
    this.selectedActivity.update((_) => process.activities?.find(() => true));
  }

  protected onSelectActivity(activity: ProcessActivityModel) {
    this.selectedActivity.update((_) => activity);
  }

  protected setShowAll(change: MatSlideToggleChange) {
    this.showAll.update((_) => change.checked);
    this.processEngineService
      .getRunningProcessesOfJob({
        jobId: this.job().model.id,
        allProcesses: this.showAll()
      })
      .then((data) => this.updateData(data))
      .catch(async (e: HttpErrorResponse) =>
        await this.snackbarService.handleError(e));
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

