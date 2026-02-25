/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Entry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { Subscription } from 'rxjs';
import { EditResourceService } from '../../../services/edit-resource.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-resource-properties',
  templateUrl: './resource-properties.html',
  styleUrls: ['./resource-properties.scss'],
  imports: [MatProgressSpinnerModule, NavigableEntryEditor]
})
export class ResourceProperties implements OnInit, OnDestroy {
  private editResourceService = inject(EditResourceService);

  isEditMode = toSignal(this.editResourceService.edit$, { initialValue: false });
  properties = signal<Entry | undefined>(undefined);
  private editServiceSubscription: Subscription | undefined;

  ngOnInit(): void {
    this.editServiceSubscription = this.editResourceService.activeResource$.subscribe(resource => {
      if (resource?.properties) {
        this.properties.update(() => resource.properties);
      }
    });
  }

  ngOnDestroy(): void {
    this.editServiceSubscription?.unsubscribe();
  }
}
