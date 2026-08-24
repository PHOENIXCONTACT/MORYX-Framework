/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, ChangeDetectionStrategy } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { TranslationConstants } from '../translation-constants';
import { SkillType } from '../models/skill-type-model';
import { getDurationInDays } from '../models/utils';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialog } from '../dialogs/confirmation-dialog/confirmation-dialog';
import { AppStoreService } from '../services/app-store.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-skill-types',
  templateUrl: './skill-types.html',
  styleUrl: './skill-types.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatTooltipModule,
    MatIconModule,
    RouterLink,
    TranslatePipe,
    MatTableModule,
    MatButtonModule,
    MatToolbarModule
  ]
})
export class SkillTypes {
  private dialog = inject(MatDialog);
  private appStoreService = inject(AppStoreService);
  private translateService = inject(TranslateService);

  protected skillTypes = this.appStoreService.skillTypes;
  protected skills = this.appStoreService.skills;

  protected getDurationInDays = getDurationInDays;
  protected dataSource!: MatTableDataSource<SkillType>;
  protected TranslationConstants = TranslationConstants;
  protected displayedColumns: string[] = ['name', 'duration', 'trainedOperators', 'actions'];

  constructor() {
    effect(() => {
      this.dataSource = new MatTableDataSource(this.skillTypes());
    });
  }

  protected async onDeleteClick(skillType: SkillType) {
    const translations = await firstValueFrom(this.translateService
      .get([
        TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TYPE_TITLE,
        TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TYPE_MESSAGE
      ]));

    const dialogRef = this.dialog.open(ConfirmationDialog, {
      data: {
        dialogMessage: translations[TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TYPE_MESSAGE],
        dialogTitle: translations[TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TYPE_TITLE],
        dialogResult: 'NO'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result.dialogResult === 'NO') {
        return;
      }

      this.appStoreService.deleteSkillType(skillType);
    });
  }

  protected operatorWithSkillCount(typeId: number) {
    return this.skills().filter(x => x.typeId === typeId).length;
  }

}

