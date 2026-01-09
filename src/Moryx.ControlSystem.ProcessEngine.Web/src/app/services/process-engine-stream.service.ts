import { Injectable, NgZone } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ProcessEngineService } from '../api/services';
import { JobProcessModel } from '../api/models/job-process-model';
import { ProcessActivityModel } from '../api/models/process-activity-model';

@Injectable({
  providedIn: 'root'
})
export class ProcessEngineStreamService {
  updatedProcess: BehaviorSubject<JobProcessModel | undefined> = new BehaviorSubject<JobProcessModel | undefined>(undefined);
  updatedActivity: BehaviorSubject<ProcessActivityModel | undefined> = new BehaviorSubject<ProcessActivityModel | undefined>(undefined);

  constructor(
    private processEngineService: ProcessEngineService,
    private ngZone: NgZone) {
    this.publishActivityUpdates();
    this.publishProcessUpdates();
  }

  private publishProcessUpdates(): void {
    const eventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.ProcessUpdatesStreamPath);
    eventSource.onmessage = event => {
      const process = JSON.parse(event.data);
      this.ngZone.run(() => this.updatedProcess.next(process));
    };
  }

  private publishActivityUpdates(): void {
    const eventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.ActivitiesUpdatesStreamPath);
    eventSource.onmessage = event => {
      const activity = JSON.parse(event.data);
      this.ngZone.run(() => this.updatedActivity.next(activity));
    };
  }

}
