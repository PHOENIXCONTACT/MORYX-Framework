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

  private readonly _updatedProcessHolderGroups = signal<ProcessHolderGroupModel | undefined>(undefined);
  readonly updatedProcessHolderGroups = this._updatedProcessHolderGroups.asReadonly();

  connect() {
    this.eventSource = new EventSource(this.processEngineService.rootUrl + ProcessEngineService.GroupStreamPath);
    this.eventSource.onmessage = event => {
      const holderGroup = JSON.parse(event.data);
      console.log('update received :', holderGroup);
      this._updatedProcessHolderGroups.set(holderGroup);
    };
  }

  disconnect() {
    if (this.eventSource) {
      this.eventSource.close();
      this.eventSource = undefined;
    }
  }
}
