/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, linkedSignal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { EditResourceService } from '../../../services/edit-resource.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Entry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';

@Component({
  selector: 'app-resource-properties',
  templateUrl: './resource-properties.html',
  styleUrls: ['./resource-properties.scss'],
  imports: [MatProgressSpinnerModule, NavigableEntryEditor]
})
export class ResourceProperties {

  private editResourceService = inject(EditResourceService);

  isEditMode = toSignal(this.editResourceService.edit$, { initialValue: false });
  properties = linkedSignal(() => this.editResourceService.activeResource()?.properties);

  propertiesChanged(properties: Entry): void {
    const resource = this.editResourceService.activeResource();
    if (!resource){
      throw new Error('Trying to update properties of a resource, but no resource is active.');
    }
    resource.properties = properties;
    this.editResourceService.updateActiveResource(resource);
  }
}
