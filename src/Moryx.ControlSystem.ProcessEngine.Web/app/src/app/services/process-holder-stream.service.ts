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
  private processEngineService = inject(ProcessEngineService);

  $updatedProcessHolderGroups = new BehaviorSubject<ProcessHolderGroupModel | undefined>(undefined);

  constructor() {
    this.publishUpdates();
  }

  publishUpdates() {
    const eventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.GroupStreamPath);
    eventSource.onmessage = event => {
      const holderGroup = JSON.parse(event.data);
      console.log('update received :', holderGroup);
      this.$updatedProcessHolderGroups.next(holderGroup);
    };
  }

}

