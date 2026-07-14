/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LanguageService } from '@moryx/ngx-web-framework/services';
import { TranslateService } from '@ngx-translate/core';
import { environment } from '../environments/environment';
import {
  ConfigurationDialog,
  DialogData,
} from './dialogs/configuration-dialog/configuration-dialog';
import './extensions/observable.extensions';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { CookieService } from './services/cookie.service';
import { InstructionService } from './services/instruction.service';

import { WorkerInstructions } from './components/worker-instructions/worker-instructions';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

const COOKIE_NAME = 'moryx-client-identifier';

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
  private languageService = inject(LanguageService);
  private destroyRef = inject(DestroyRef);

  protected environment = environment;
  protected clientIdentifier: string = '';

  constructor() {
    const cookie = this.cookieService.getCookie(COOKIE_NAME);
    if (cookie) {
      this.clientIdentifier = cookie;
    } else {
      this.openConfigDialog();
    }

    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translateService.setFallbackLang('en');
    this.translateService.use(this.languageService.getFallbackLang());
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
    this.cookieService.setCookie(COOKIE_NAME, result.instructorName, 365);

    this.instructionService.disconnect();
    this.instructionService.connect();
  }

  private async showNoInstructorWarning(): Promise<void> {
    const snackbarTexts = await this.translateService
      .get([
        TranslationConstants.APP.NO_INSTRUCTOR_MESSAGE,
        TranslationConstants.DISMISS,
      ])
      .toAsync();

    this.snackBar.open(
      snackbarTexts[TranslationConstants.APP.NO_INSTRUCTOR_MESSAGE],
      snackbarTexts[TranslationConstants.DISMISS],
      {
        panelClass: ['error'],
        duration: 5000,
      }
    );
  }

  disconnectEvents(): void {
    this.instructionService.disconnect();
  }
}

