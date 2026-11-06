/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, linkedSignal, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, NavigationCancel, NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { ResourceModel } from '../../api/models';
import { EditResourceService } from '@app/services/edit-resource.service';
import { ResourceNavigationService } from '@app/services/resource-navigation.service';
import { SessionService } from '@app/services/session.service';
import { ResourceModificationService } from '@api/services';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DetailsHeader } from './details-header/details-header';

@Component({
  selector: 'app-details-view',
  templateUrl: './details-view.html',
  styleUrls: ['./details-view.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [RouterOutlet, TranslatePipe, MatTabsModule, MatProgressSpinnerModule, RouterLink, DetailsHeader]
})
export class DetailsView {
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private editResourceService = inject(EditResourceService);
  private navigationService = inject(ResourceNavigationService);
  private resourceModificationService = inject(ResourceModificationService);
  private sessionService = inject(SessionService);
  private snackbarService = inject(SnackbarService);

  protected loading = this.navigationService.loading;
  protected isEditMode = this.editResourceService.editing;
  protected activeLink = signal<number | undefined>(undefined);
  protected activeResource = linkedSignal(() => this.editResourceService.activeResource());

  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.activatedRoute.paramMap.subscribe(params => {
      this.loadResource(Number(params.get('id')));
    });

    this.router.events.subscribe(val => {
      if (val instanceof NavigationEnd || val instanceof NavigationCancel) {
        const url = this.router.url;
        if (url.includes('/references')) {
          this.activeLink.set(2);
        } else if (url.includes('/methods')) {
          this.activeLink.set(3);
        } else {
          this.activeLink.set(1);
        }
      }
    });

    effect(() => {
      const resource = this.activeResource();
      if (!resource) {
        return;
      }
      if (this.navigationService.activeId() === resource.id) {
        return;
      }
      untracked(() => {
        this.onNewResource(resource);
      });
    });
  }

  private async loadResource(id: number) {
    if (this.navigationService.activeId() === id) {
      return;
    }
    this.navigationService.setActiveId(id);
    this.navigationService.setLoading(true);

    // Restore work-in-progress resource from session storage
    const workInProgress = this.sessionService.removeWipResource();
    if (workInProgress?.resource.id === id) {
      this.editResourceService.setResourceFromStorage(workInProgress);
      this.navigationService.setLoading(false);
      return;
    }

    // Creating a new resource — edit service already holds the resource
    if (id === 0) {
      const resource = this.editResourceService.activeResource();
      if (!resource) {
        this.router.navigate(['']);
      }
      this.navigationService.setLoading(false);
      return;
    }

    try {
      const resource = await this.resourceModificationService.getDetails({ id });
      this.editResourceService.setResource(resource);
    } catch (error) {
      await this.snackbarService.handleError(error as HttpErrorResponse);
      this.router.navigate(['']);
    } finally {
      this.navigationService.setLoading(false);
    }
  }

  private onNewResource(resource: ResourceModel) {
    const url = this.router.url;
    if (url.includes('/methods')) {
      this.router.navigate([`details/${resource?.id}/methods`]);
    } else if (url.includes('/references')) {
      this.router.navigate([`details/${resource?.id}/references`]);
    } else if (resource?.properties) {
      this.router.navigate([`details/${resource.id}/properties`]);
    }
  }
}
