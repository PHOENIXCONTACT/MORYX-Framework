/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, linkedSignal, signal, untracked } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Event, NavigationCancel, NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ResourceModel } from '../../api/models';
import { EditResourceService } from '../../services/edit-resource.service';
import { MatTabsModule } from '@angular/material/tabs';
import { DetailsHeader } from './details-header/details-header';

@Component({
  selector: 'app-details-view',
  templateUrl: './details-view.html',
  styleUrls: ['./details-view.scss'],
  imports: [RouterOutlet, TranslateModule, MatTabsModule, RouterLink, DetailsHeader]
})
export class DetailsView {
  private router = inject(Router);
  private editResourceService = inject(EditResourceService);

  isEditMode = toSignal(this.editResourceService.edit$, { initialValue: false });
  activeLink = signal<number | undefined>(undefined);
  activeResource = linkedSignal(() => this.editResourceService.activeResource());

  TranslationConstants = TranslationConstants;

  private oldResourceId?: number;

  constructor() {
    this.router.events.subscribe(event => this.onRoutingEvent(event));
    effect(() => {
      const resource = this.activeResource();
      if (!resource) return;
      if (this.oldResourceId === resource.id) return;
      untracked(() => this.onNewResource(resource));
    });
  }

  private onNewResource(resource: ResourceModel) {
    const url = this.router.url;
    const regexMethods: RegExp = /(details\/\d*\/methods)/;
    const regexReferences: RegExp = /(details\/\d*\/references)/;
    if (regexMethods.test(url)) {
      this.router.navigate([`details/${resource?.id}/methods`]);
    } else if (regexReferences.test(url)) {
      this.router.navigate([`details/${resource?.id}/references`]);
    } else if (resource?.properties) {
      this.router.navigate([`details/${resource.id}/properties`]);
    }
    this.oldResourceId = resource?.id;
  }

  private onRoutingEvent(event: Event) {
    if (event instanceof NavigationEnd || event instanceof NavigationCancel) {
      let url = this.router.url;
      const regexProperty: RegExp = /(details\/\d*\/properties)/;
      const regexReferences: RegExp = /(details\/\d*\/references)/;
      const regexMethods: RegExp = /(details\/\d*\/methods)/;
      if (regexProperty.test(url)) {
        this.activeLink.set(1);
      } else if (regexReferences.test(url)) {
        this.activeLink.set(2);
      } else if (regexMethods.test(url)) {
        this.activeLink.set(3);
      }
    }
  }
}
