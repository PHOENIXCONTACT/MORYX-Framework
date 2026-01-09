import { Injectable, NgZone } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ProcessHolderGroupModel } from '../api/models/process-holder-group-model';
import { ProcessEngineService } from '../api/services';

@Injectable({
  providedIn: 'root'
})
export class ProcessHolderStreamService {

  $updatedWpc = new BehaviorSubject<ProcessHolderGroupModel | undefined>(undefined);

  constructor(private ngZone: NgZone,
    private processService: ProcessEngineService
  ) {
    this.publishUpdates();
  }

  publishUpdates() {
    const eventSource = new EventSource(this.processService.rootUrl + ProcessEngineService.GroupStreamPath);
    eventSource.onmessage = event => {
      const wpcGroup = JSON.parse(event.data);
      this.ngZone.run(() => {
        console.log('update received :', wpcGroup);
        this.$updatedWpc.next(wpcGroup)
      });
    };
  }

}
