/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { ConfigurationDialog, DialogData } from './dialogs/configuration-dialog/configuration-dialog';
import { TranslationConstants } from './translation-constants';
import { CookieService } from './services/cookie.service';
import { InstructionService } from './services/instruction.service';
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
export class App implements OnInit {
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private translateService = inject(TranslateService);
  private cookieService = inject(CookieService);
  private instructionService = inject(InstructionService);
  private destroyRef = inject(DestroyRef);

  protected environment = environment;
  protected clientIdentifier: string = '';

  private readonly cookieName = 'moryx-client-identifier';
  private readonly cookieLifetimeDays = 365;

  constructor() {
    const cookie = this.cookieService.getCookie(this.cookieName);
    if (cookie) {
      this.clientIdentifier = cookie;
      this.cookieService.setCookie(this.cookieName, cookie, this.cookieLifetimeDays);
    } else {
      this.openConfigDialog();
    }

    this.destroyRef.onDestroy(() => this.disconnectEvents());
  }

  ngOnInit(): void {
    this.instructionService.connect();
  }

  protected openConfigDialog(): void {
    const dialogRef = this.dialog.open(ConfigurationDialog, {
      data: <DialogData>{
        instructorName: this.clientIdentifier,
      }
    });

    dialogRef
      .afterClosed()
      .subscribe(result => this.handleDialogResult(result));
  }

  private async handleDialogResult(result: DialogData | undefined) {
    if (result?.instructorName?.length) {
      this.updateInstructor(result);
    }

    if (!this.clientIdentifier) {
      await this.showNoInstructorWarning();
    }
  }

  private updateInstructor(result: DialogData): void {
    this.clientIdentifier = result.instructorName;
    this.cookieService.setCookie(this.cookieName, result.instructorName, this.cookieLifetimeDays);

    this.instructionService.disconnect();
    this.instructionService.connect();
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

