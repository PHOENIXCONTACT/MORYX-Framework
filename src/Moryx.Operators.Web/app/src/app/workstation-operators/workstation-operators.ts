/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, inject, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { WorkstationViewModel } from '../models/workstation-view-model';
import { WorkstationTogglingState } from './WorkstationTogglingState';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';
import { AddOperatorDialog } from '../dialogs/add-operator/add-operator';
import { TranslationConstants } from '../translation-constants';
import { skillTypeToModel } from '../models/model-converter';
import { OperatorViewModel } from '../models/operator-view-model';
import { AppStoreService } from '../services/app-store.service';
import { Operators } from '../operators/operators';

import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { TranslatePipe } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-workstation-operators',
  templateUrl: './workstation-operators.html',
  styleUrl: './workstation-operators.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    Operators,
    MatTooltipModule,
    MatIconModule,
    EmptyState,
    TranslatePipe,
    MatButtonModule,
    MatToolbarModule,
    MatCardModule,
    RouterLink
  ]
})
export class WorkstationOperators {
  private appStoreService = inject(AppStoreService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);

  protected workstations = this.appStoreService.workstations;
  protected workstationTogglingState = signal<WorkstationTogglingState | undefined>(undefined);
  protected operatorsSkills = this.appStoreService.skills;
  protected skillTypes = computed(() => this.appStoreService.skillTypes().map(skillTypeToModel));
  protected isCardExpanded = computed(() => {
    if (!this.workstationTogglingState()) {
      return false;
    }

    return this.workstationTogglingState()?.isExpanded;
  })

  protected TranslationConstants = TranslationConstants;

  constructor() {
    // Restore expanded card from URL on first load
    effect(() => {
      const stations = this.workstations();
      if (stations.length) {
        untracked(() => {
          this.expandPreviousCard(stations)
        });
      }
    });
  }

  private expandPreviousCard(stations: WorkstationViewModel[]) {
    //make sure expand the previously expanded card if exist in the URL

    // ie : /?stationId=2
    const urlFragments = this.router.url.split('?');// ['/', 'stationId=2']
    if (urlFragments.length === 0) {
      return;
    }

    const stationIdUrl = urlFragments[urlFragments.length - 1].split('=')[1]// ['stationId', '2']
    const stationId = Number(stationIdUrl);
    const station = stations.find((x) => x.data.id === stationId);
    if (!stationId || !station) {
      return;
    }

    //expand this workstation card
    this.workstationTogglingState.set(<WorkstationTogglingState>{
      station,
      isExpanded: true
    });
  }

  protected toggleWorkstationCard(station: WorkstationViewModel | undefined) {
    this.workstationTogglingState.set(<WorkstationTogglingState>{
      station,
      isExpanded: !this.workstationTogglingState()?.isExpanded
    });
    if (this.workstationTogglingState()?.isExpanded) {
      this.updateUrlParam(station?.data.id ?? null);
    } else {
      this.updateUrlParam(null);
    }
  }

  protected addOperator() {
    const dialogResult = this.dialog.open(AddOperatorDialog);
    //navigate to operator details
    dialogResult.afterClosed()
      .subscribe((result: OperatorViewModel) =>
        setTimeout(() => this.router.navigate(['/management/operator/details/', `${result.data.identifier}`]), 500)
      );
  }

  private updateUrlParam(stationId: number | null) {
    const queryParams: Params = {stationId: stationId};

    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: queryParams
    });
  }
}

