/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { VisualInstructionsService } from '@app/api/services/visual-instructions.service';
import { TranslationConstants } from '@app/translation-constants';
import { FocusMode, InstructionStateService } from '@app/services/instruction-state.service';

@Component({
  selector: 'app-settings-dialog',
  templateUrl: './settings-dialog.html',
  styleUrls: ['./settings-dialog.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatSelectionList,
    MatListOption,
    MatDialogModule,
    TranslatePipe,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatButtonModule
  ]
})
export class SettingsDialog implements OnInit {
  private dialogRef = inject(MatDialogRef<SettingsDialog>);
  private visualInstructionsService = inject(VisualInstructionsService);
  private snackbarService = inject(SnackbarService);
  private instructionStateService = inject(InstructionStateService);

  protected instructors = signal<string[]|undefined>(undefined);
  protected selectedInstructor = signal(this.instructionStateService.instructor());
  protected selectedFocusMode = signal(this.instructionStateService.focusMode());

  protected TranslationConstants = TranslationConstants;
  protected FocusMode = FocusMode;

  ngOnInit(): void {
    this.visualInstructionsService.getInstructors()
      .then((result) => this.instructors.set(result.sort((a, b) => a.localeCompare(b))))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  protected selectInstructor(name: string): void {
    this.selectedInstructor.set(name);
  }

  protected selectFocusMode(mode: FocusMode): void {
    this.selectedFocusMode.set(mode);
  }

  protected apply(): void {
    this.instructionStateService.setFocusMode(this.selectedFocusMode());
    this.instructionStateService.setInstructor(this.selectedInstructor());
    this.dialogRef.close(true);
  }
}
