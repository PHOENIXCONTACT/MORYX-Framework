/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ChangeDetectionStrategy, DestroyRef, effect, untracked } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { SettingsDialog } from './dialogs/settings-dialog/settings-dialog';
import { TranslationConstants } from './translation-constants';
import { InstructionService } from './services/instruction.service';
import { InstructionStateService } from './services/instruction-state.service';
import { WorkerInstructions } from './components/worker-instructions/worker-instructions';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    WorkerInstructions,
    MatButtonModule,
    MatIconModule
  ],
  host: {
    '(window:beforeunload)': 'disconnectEvents()'
  }
})
export class App {
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private translateService = inject(TranslateService);
  private instructionService = inject(InstructionService);
  private instructionStateService = inject(InstructionStateService);
  private destroyRef = inject(DestroyRef);

  protected clientIdentifier = this.instructionStateService.instructor;

  constructor() {
    if (!this.clientIdentifier()) {
      this.openSettingsDialog();
    }

    effect(() => {
      const instructor = this.clientIdentifier();
      untracked(() => {
        this.instructionService.disconnect();
        if (instructor) {
          this.instructionService.connect();
        }
      });
    });

    this.destroyRef.onDestroy(() => this.disconnectEvents());
  }

  protected openSettingsDialog(): void {
    const dialogRef = this.dialog.open(SettingsDialog);

    dialogRef
      .afterClosed()
      .subscribe(result => this.handleDialogResult(result));
  }

  private async handleDialogResult(result: boolean | undefined) {
    if (!this.clientIdentifier()) {
      await this.showNoInstructorWarning();
    }
  }

  private async showNoInstructorWarning(): Promise<void> {
    const snackbarTexts = await firstValueFrom(this.translateService
      .get([
        TranslationConstants.APP.NO_INSTRUCTOR_MESSAGE,
        TranslationConstants.DISMISS,
      ]));

    this.snackBar.open(
      snackbarTexts[TranslationConstants.APP.NO_INSTRUCTOR_MESSAGE],
      snackbarTexts[TranslationConstants.DISMISS],
      {
        panelClass: ['error'],
        duration: 5000,
      }
    );
  }

  protected disconnectEvents(): void {
    this.instructionService.disconnect();
  }
}
