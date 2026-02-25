/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslateModule } from '@ngx-translate/core';
import { VisualInstructionsService } from 'src/app/api/services/visual-instructions.service';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';

@Component({
  selector: 'app-configuration-dialog',
  templateUrl: './configuration-dialog.html',
  styleUrls: ['./configuration-dialog.scss'],
  imports: [
    MatSelectionList,
    MatListOption,
    MatDialogModule,
    TranslateModule,
    MatProgressSpinnerModule
  ]
})
export class ConfigurationDialog implements OnInit {
  data = inject<DialogData>(MAT_DIALOG_DATA);
  private visualInstructionsService = inject(VisualInstructionsService);
  private snackbarService = inject(SnackbarService);

  instructors = signal<string[]|undefined>(undefined);
  TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.visualInstructionsService.getInstructors().subscribe({
      next: (result) => this.instructors.update(_ => result.sort((a, b) => a.localeCompare(b))),
      error: async (e: HttpErrorResponse) =>
        await this.snackbarService.handleError(e)
    });
  }

  saveName(name: string) {
    this.data.instructorName = name;
  }
}

export interface DialogData {
  instructorName: string;
}

