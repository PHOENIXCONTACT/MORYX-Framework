/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { ProcessEngineService } from '@api/services';
import { JobProcessModel } from '@api/models/job-process-model';
import { ProcessActivityModel } from '@api/models/process-activity-model';

@Injectable({
  providedIn: 'root'
})
export class ProcessEngineStreamService {
  private processEngineService = inject(ProcessEngineService);
  private processEventSource?: EventSource;
  private activitiesEventSource?: EventSource;

  private readonly _updatedProcess = signal<JobProcessModel | undefined>(undefined);
  readonly updatedProcess = this._updatedProcess.asReadonly();
  private readonly _updatedActivity = signal<ProcessActivityModel | undefined>(undefined);
  readonly updatedActivity = this._updatedActivity.asReadonly();

  connect() {
    this.publishActivityUpdates();
    this.publishProcessUpdates();
  }

  private publishProcessUpdates(): void {
    this.processEventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.ProcessUpdatesStreamPath);
    this.processEventSource.onmessage = event => {
      const process = JSON.parse(event.data);
      this._updatedProcess.set(process);
    };
  }

  private publishActivityUpdates(): void {
    this.activitiesEventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.ActivitiesUpdatesStreamPath);
    this.activitiesEventSource.onmessage = event => {
      const activity = JSON.parse(event.data);
      this._updatedActivity.set(activity);
    };
  }

  disconnect() {
    if (this.processEventSource) {
      this.processEventSource.close();
      this.processEventSource = undefined;
    }

    if (this.activitiesEventSource) {
      this.activitiesEventSource.close();
      this.activitiesEventSource = undefined;
    }
  }
}
