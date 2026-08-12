/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { VisualInstructionsService } from '@app/api/services/visual-instructions.service';
import { TranslationConstants } from '@app/translation-constants';

@Component({
  selector: 'app-configuration-dialog',
  templateUrl: './configuration-dialog.html',
  styleUrls: ['./configuration-dialog.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatSelectionList,
    MatListOption,
    MatDialogModule,
    TranslatePipe,
    MatProgressSpinnerModule
  ]
})
export class ConfigurationDialog implements OnInit {
  protected data = inject<DialogData>(MAT_DIALOG_DATA);
  private visualInstructionsService = inject(VisualInstructionsService);
  private snackbarService = inject(SnackbarService);

  protected instructors = signal<string[]|undefined>(undefined);
  protected TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.visualInstructionsService.getInstructors()
      .then((result) => this.instructors.set(result.sort((a, b) => a.localeCompare(b))))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  protected saveName(name: string) {
    this.data.instructorName = name;
  }
}

export interface DialogData {
  instructorName: string;
}

