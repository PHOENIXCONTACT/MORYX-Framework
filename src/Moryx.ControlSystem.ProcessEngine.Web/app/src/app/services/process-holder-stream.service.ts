/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { ProcessHolderGroupModel } from '@api/models/process-holder-group-model';
import { ProcessEngineService } from '@api/services';

@Injectable({
  providedIn: 'root'
})
export class ProcessHolderStreamService {
  private processEngineService = inject(ProcessEngineService);
  private eventSource?: EventSource;

  updatedProcessHolderGroups = signal<ProcessHolderGroupModel | undefined>(undefined);

  connect() {
    this.eventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.GroupStreamPath);
    this.eventSource.onmessage = event => {
      const holderGroup = JSON.parse(event.data);
      console.log('update received :', holderGroup);
      this.updatedProcessHolderGroups.set(holderGroup);
    };
  }

  disconnect() {
    if (this.eventSource) {
      this.eventSource.close();
      this.eventSource = undefined;
    }
  }
}
