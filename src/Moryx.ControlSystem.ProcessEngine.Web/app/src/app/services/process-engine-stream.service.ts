/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ProcessEngineService } from '@api/services';
import { JobProcessModel } from '@api/models/job-process-model';
import { ProcessActivityModel } from '@api/models/process-activity-model';

@Injectable({
  providedIn: 'root'
})
export class ProcessEngineStreamService {
  private processEngineService = inject(ProcessEngineService);
  private processEventSource: EventSource | null = null;
  private activitiesEventSource: EventSource | null = null;

  updatedProcess: BehaviorSubject<JobProcessModel | undefined> = new BehaviorSubject<JobProcessModel | undefined>(undefined);
  updatedActivity: BehaviorSubject<ProcessActivityModel | undefined> = new BehaviorSubject<ProcessActivityModel | undefined>(undefined);

  connect() {
    this.publishActivityUpdates();
    this.publishProcessUpdates();
  }

  private publishProcessUpdates(): void {
    this.processEventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.ProcessUpdatesStreamPath);
    this.processEventSource.onmessage = event => {
      const process = JSON.parse(event.data);
      this.updatedProcess.next(process);
    };
  }

  private publishActivityUpdates(): void {
    this.activitiesEventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.ActivitiesUpdatesStreamPath);
    this.activitiesEventSource.onmessage = event => {
      const activity = JSON.parse(event.data);
      this.updatedActivity.next(activity);
    };
  }

  disconnect() {
    if (this.processEventSource) {
      this.processEventSource.close();
      this.processEventSource = null;
    }

    if (this.activitiesEventSource) {
      this.activitiesEventSource.close();
      this.activitiesEventSource = null;
    }
  }
}

