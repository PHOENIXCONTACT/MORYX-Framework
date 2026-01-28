/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { Entry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { Subscription } from 'rxjs';
import { EditResourceService } from '../../../services/edit-resource.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';


@Component({
  selector: 'app-resource-properties',
  templateUrl: './resource-properties.component.html',
  styleUrls: ['./resource-properties.component.scss'],
  imports: [MatProgressSpinnerModule, NavigableEntryEditor],
  standalone: true,
})
export class ResourcePropertiesComponent implements OnInit, OnDestroy {
  properties = signal<Entry | undefined>(undefined);
  private editServiceSubscription: Subscription | undefined;

  constructor(public editService: EditResourceService) {
  }

  ngOnInit(): void {
    this.editServiceSubscription = this.editService.activeResource$.subscribe(resource => {
      if (resource?.properties) {
        this.properties.update(() => resource.properties);
      }
    });
  }

  ngOnDestroy(): void {
    this.editServiceSubscription?.unsubscribe();
  }
}

