/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, NgZone } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { JobManagementService } from '../api/services';
import { JobModel } from '../api/models/job-model';

@Injectable({
  providedIn: 'root'
})
export class JobManagementStreamService {
  updatedJob: BehaviorSubject<JobModel | undefined> = new BehaviorSubject<JobModel | undefined>(undefined);

  constructor(
    private jobManagementService: JobManagementService,
    private ngZone: NgZone) {

    const eventSource = new EventSource(this.jobManagementService.rootUrl + JobManagementService.ProgressStreamPath);
    eventSource.onmessage = event => {
      const job = <JobModel>JSON.parse(event.data);
      this.ngZone.run(() => this.publishUpdate(job));
    };
  }

  private publishUpdate(job: JobModel): void {
    if (Object.keys(job).length > 0) {
      this.updatedJob.next(job);
    }
    else {
      this.updatedJob.next(undefined);
    }
  }

}

