/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, signal, ChangeDetectionStrategy } from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { OperatorViewModel } from "../models/operator-view-model";
import { MatDialog } from "@angular/material/dialog";
import { ConfirmationDialog } from "../dialogs/confirmation-dialog/confirmation-dialog";
import { AddOperatorDialog } from "../dialogs/add-operator/add-operator";
import { TranslationConstants } from "../translation-constants";
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { skillTypeToModel } from "../models/model-converter";
import { Router, RouterLink } from "@angular/router";
import { AppStoreService } from "../services/app-store.service";

import { MatTooltipModule } from "@angular/material/tooltip";
import { MatIconModule } from "@angular/material/icon";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatExpansionModule } from "@angular/material/expansion";
import { OperatorSkillChips } from "../operator-skill-chips/operator-skill-chips";
import { MatButtonModule } from "@angular/material/button";
import { MatToolbarModule } from "@angular/material/toolbar";

@Component({
  selector: "app-operators-management",
  templateUrl: "./operators-management.html",
  styleUrl: "./operators-management.scss",
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatTooltipModule,
    MatIconModule,
    MatSidenavModule,
    MatExpansionModule,
    OperatorSkillChips,
    RouterLink,
    TranslatePipe,
    MatButtonModule,
    MatToolbarModule
  ]
})
export class OperatorsManagement {
  private appStoreService = inject(AppStoreService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private translateService = inject(TranslateService);

  protected operators = this.appStoreService.operators;
  private translations = toSignal(this.translateService.get([
    TranslationConstants.OPERATORS_MANAGEMENT.DELETE_TITLE,
    TranslationConstants.OPERATORS_MANAGEMENT.DELETE_MESSAGE,
  ]));
  protected deleteDialogTitle = computed(() => this.translations()?.[TranslationConstants.OPERATORS_MANAGEMENT.DELETE_TITLE] ?? '');
  protected deleteDialogMessage = computed(() => this.translations()?.[TranslationConstants.OPERATORS_MANAGEMENT.DELETE_MESSAGE] ?? '');
  protected inMenuMode = signal(false);
  protected skills = this.appStoreService.skills;
  protected skillTypes = computed(() => this.appStoreService.skillTypes().map(skillTypeToModel));

  protected TranslationConstants = TranslationConstants;

  protected updateMenuMode(value: boolean) {
    this.inMenuMode.set(value);
  }

  protected onDeleteClick(operator: OperatorViewModel) {

    const dialogRef = this.dialog.open(ConfirmationDialog, {
      data: {
        dialogMessage: this.deleteDialogMessage,
        dialogTitle: this.deleteDialogTitle,
        dialogResult: 'NO'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result.dialogResult === 'NO') {
        return;
      }

      this.appStoreService.deleteOperator(operator);
    });
  }

  protected onAddClick() {
    const dialogResult = this.dialog.open(AddOperatorDialog);
    //navigate to operator details
    dialogResult.afterClosed()
      .subscribe((result: OperatorViewModel) =>
        setTimeout(() => this.router.navigate(['/management/operator/details/', `${result.data.identifier}`]), 500)
      );
  }

  protected getSkillsForOperator(id: string) {
    return this.skills().filter(x => x.operatorId === id);
  }
}

