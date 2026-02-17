/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ProcessHolderGroupModel } from '../api/models/process-holder-group-model';
import { ProcessEngineService } from '../api/services';

@Injectable({
  providedIn: 'root'
})
export class ProcessHolderStreamService {
  private processService = inject(ProcessEngineService);

  $updatedWpc = new BehaviorSubject<ProcessHolderGroupModel | undefined>(undefined);

  constructor() {
    this.publishUpdates();
  }

  publishUpdates() {
    const eventSource = new EventSource(this.processService.rootUrl + ProcessEngineService.GroupStreamPath);
    eventSource.onmessage = event => {
      const wpcGroup = JSON.parse(event.data);
      console.log('update received :', wpcGroup);
      this.$updatedWpc.next(wpcGroup);
    };
  }

}

