/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, inject, input, signal, untracked, ChangeDetectionStrategy } from "@angular/core";
import { RouterLink } from "@angular/router";
import { TranslationConstants } from "../translation-constants";
import { OperatorSkill } from "../models/operator-skill-model";
import { dateToString } from "../models/utils";
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { MatDialog } from "@angular/material/dialog";
import { SkillNewDialog } from "../dialogs/skill-new-dialog/skill-new-dialog";
import { ConfirmationDialog } from "../dialogs/confirmation-dialog/confirmation-dialog";
import { OperatorViewModel } from "../models/operator-view-model";
import { AssignableOperator } from "@api/models/assignable-operator";
import { skillToOperatorSkill, skillTypeToModel } from "../models/model-converter";
import { firstValueFrom } from "rxjs";
import { AppStoreService } from "../services/app-store.service";
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { MatIconModule } from "@angular/material/icon";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { FormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatToolbarModule } from "@angular/material/toolbar";

@Component({
  selector: "app-operator-details",
  templateUrl: "./operator-details.html",
  styleUrl: "./operator-details.scss",
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatIconModule,
    MatSidenavModule,
    MatTooltipModule,
    TranslatePipe,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatToolbarModule,
    RouterLink
  ]
})
export class OperatorDetails {
  private appStoreService = inject(AppStoreService);
  private dialog = inject(MatDialog);
  private translateService = inject(TranslateService);

  readonly id = input.required<string>();
  protected editMode = signal(false);
  protected operator = signal<AssignableOperator>({
    assignedResources: [],
    firstName: '',
    identifier: '',
    lastName: '',
    pseudonym: '',
    signedIn: false
  });
  skillTypes = computed(() => this.appStoreService.skillTypes().map(skillTypeToModel));
  operatorViewModel = signal<OperatorViewModel | undefined>(undefined);

  protected TranslationConstants = TranslationConstants;
  protected dateToString = dateToString;
  protected dataSource!: MatTableDataSource<OperatorSkill>;
  protected displayedColumns: string[] = ['type', 'obtainedOn', 'expiresOn', 'actions'];

  constructor() {
    effect(() => {
      const id = this.id();
      untracked(() => {
        this.initialize(id);
      });
    });
  }

  private initialize(id: string) {
    const identifier = id;
    if (!identifier) {
      return;
    }
    const operatorDataPromise = this.appStoreService.getOperator(identifier);

    operatorDataPromise.then(result => {
      if (!result) {
        return;
      }

      this.operatorViewModel.set(result);
      this.operator.set(result.data);
    });


    this.loadSkills();
  }

  private loadSkills() {
    this.appStoreService.getSkillFromRemoteSource()
      .then(skills => {
        const skillModels = skills.filter(e => e.operatorIdentifier === this.operator().identifier).map(skillToOperatorSkill);
        this.dataSource = new MatTableDataSource(skillModels);
      });
  }

  protected onStopEditing() {
    this.operator.set(this.appStoreService.cancelEditing(this.operator()));
    this.editMode.set(false);
  }

  protected onStartEditing() {
    this.editMode.set(true);
  }

  protected async onSave() {
    await this.appStoreService.updateOperator(this.operator())
      .then(() => {
        this.editMode.set(false);
      });
  }

  protected applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  protected onAddSkillClick() {
    const dialogResult = this.dialog.open(SkillNewDialog, {
      width: '400px',
      data: <OperatorSkill>{
        operatorId: this.operator().identifier
      }
    });

    dialogResult.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      this.appStoreService.addSkill(this.operatorViewModel()!, result);
      setTimeout(() => this.loadSkills(), 500);
    });
  }


  protected async onDeleteSkillClick(skill: OperatorSkill) {
    const translations = await firstValueFrom(this.translateService
      .get([
        TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TITLE,
        TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_MESSAGE
      ]));
    const dialogRef = this.dialog.open(ConfirmationDialog, {
      data: {
        dialogMessage: translations[TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_MESSAGE],
        dialogTitle: translations[TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TITLE],
        dialogResult: 'NO'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result.dialogResult === 'NO') {
        return;
      }

      this.appStoreService.deleteSkill(skill);
      setTimeout(() => this.loadSkills(), 500);
    });
  }

  protected findSkillTypeById(id: number) {
    return this.skillTypes().find(x => x.id === id);
  }
}

