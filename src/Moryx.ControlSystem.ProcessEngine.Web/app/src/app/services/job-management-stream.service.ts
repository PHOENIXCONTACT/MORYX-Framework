/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { JobManagementService } from '@api/services';
import { JobModel } from '@api/models/job-model';

@Injectable({
  providedIn: 'root'
})
export class JobManagementStreamService {
  private jobManagementService = inject(JobManagementService);

  private eventSource?: EventSource;
  private readonly _updatedJob = signal<JobModel | undefined>(undefined);
  readonly updatedJob = this._updatedJob.asReadonly();

  connect() {
    this.eventSource = new EventSource(this.jobManagementService.rootUrl + JobManagementService.ProgressStreamPath);
    this.eventSource.onmessage = event => {
      const job = <JobModel>JSON.parse(event.data);
      this.publishUpdate(job);
    };
  }

  private publishUpdate(job: JobModel): void {
    if (Object.keys(job).length > 0) {
      this._updatedJob.set(job);
    }
    else {
      this._updatedJob.set(undefined);
    }
  }

  disconnect() {
    if (this.eventSource) {
      this.eventSource.close();
      this.eventSource = undefined;
    }
  }
}
