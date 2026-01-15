/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, signal } from '@angular/core';
import { WorkstationViewModel } from '../models/workstation-view-model';
import { WorkstationTogglingState } from './WorkstationTogglingState';
import { environment } from 'src/environments/environment';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';
import { AddOperatorComponentDialog } from '../dialogs/add-operator/add-operator.component';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { OperatorSkill } from '../models/operator-skill-model';
import { SkillTypeModel } from '../api/models/skill-type-model';
import { skillTypeToModel } from '../models/model-converter';
import { OperatorViewModel } from '../models/operator-view-model';
import { AppStoreService } from '../services/app-store.service';
import { CommonModule } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { EmptyStateComponent } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { OperatorsComponent } from '../operators/operators.component';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-workstation-operators',
    templateUrl: './workstation-operators.component.html',
    styleUrl: './workstation-operators.component.scss',
    standalone: true,
    imports: [
      CommonModule,
      MatTooltipModule,
      MatIconModule,
      EmptyStateComponent,
      TranslateModule,
      OperatorsComponent,
      MatButtonModule,
      RouterLink
    ]
})
export class WorkstationOperatorsComponent {

  workstations = signal<WorkstationViewModel[]>([]);
  workstationTogglingState = signal<WorkstationTogglingState | undefined>(undefined);
  inMenuMode = signal(false);
  operatorsSkills = signal<OperatorSkill[]>([]);
  skillTypes = signal<SkillTypeModel[]>([]);
  isCardExpanded = computed(() => {
    if (!this.workstationTogglingState())
          return false;
    
        return this.workstationTogglingState()?.isExpanded;
  })
  
  TranslationConstants = TranslationConstants;
  skillTypeToModel = skillTypeToModel;
  resourceToolbarImage = environment.assets + 'assets/resource-toolbar.jpg';
  
  constructor(
    private appStoreService: AppStoreService,
    public dialog: MatDialog,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.appStoreService.workstations$.subscribe((stations) => {
      this.workstations.update(_=> stations);
      if (stations.length) this.expandPreviousCard(this.workstations());
    });

    this.appStoreService.skills$.subscribe(skills => this.operatorsSkills.update(_=> skills));
    this.appStoreService.skillTypes$.subscribe(types => this.skillTypes.update(_=> types.map(skillTypeToModel)));
  }

  expandPreviousCard(stations: WorkstationViewModel[]) {
    //make sure expand the previously expanded card if exist in the URL

    // ie : /?stationId=2
    const urlFragments = this.router.url.split('?');// ['/', 'stationId=2']
    if (urlFragments.length === 0) return;

    const stationIdUrl = urlFragments[urlFragments.length - 1].split('=')[1]// ['stationId', '2']
    const stationId = Number(stationIdUrl);
    const station = stations.find((x) => x.data.id === stationId);
    if (!stationId || !station) return;

    //expand this workstation card
    this.workstationTogglingState.update(_=> <WorkstationTogglingState>{
      station,
      isExpanded: true,
    });
  }

  toggleWorkstationCard(station: WorkstationViewModel | undefined) {
    this.workstationTogglingState.update(_=> <WorkstationTogglingState>{
      station,
      isExpanded: !this.workstationTogglingState()?.isExpanded,
    });
    if (this.workstationTogglingState()?.isExpanded)
      this.updateUrlParam(station?.data.id ?? null);
    else this.updateUrlParam(null);
  }

  addOperator() {
    const dialogResult = this.dialog.open(AddOperatorComponentDialog);
    //navigate to operator details
    dialogResult.afterClosed()
    .subscribe((result: OperatorViewModel) => 
      setTimeout(() => this.router.navigate(['/management/operator/details/',`${result.data.identifier}`]),500)
    );
  }

  updateUrlParam(stationId: number | null) {
    const queryParams: Params = { stationId: stationId };

    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: queryParams,
    });

  }
}

