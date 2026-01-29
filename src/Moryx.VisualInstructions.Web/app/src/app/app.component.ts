/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LanguageService } from '@moryx/ngx-web-framework';
import { TranslateService } from '@ngx-translate/core';
import { environment } from 'src/environments/environment';
import {
  ConfigurationDialogComponent,
  DialogData,
} from './dialogs/configuration-dialog/configuration-dialog.component';
import './extensions/observable.extensions';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { CookieService } from './services/cookie.service';
import { InstructionService } from './services/instruction.service';

import { WorkerInstructionsComponent } from './components/worker-instructions/worker-instructions.component';
import { MatButtonModule } from '@angular/material/button';

const COOKIE_NAME = 'moryx-client-identifier';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    imports: [
    WorkerInstructionsComponent,
    MatButtonModule
],
    standalone: true
})
export class AppComponent implements OnInit {
  environment = environment;
  clientIdentifier: string = '';

  constructor(
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    public translate: TranslateService,
    private cookieService: CookieService,
    private instructionService: InstructionService,
    private languageService: LanguageService
  ) {
    const cookie = this.cookieService.getCookie(COOKIE_NAME);
    if (cookie) {
      this.clientIdentifier = cookie;
    } else {
      this.openConfigDialog();
    }

    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  ngOnInit(): void {}

  openConfigDialog(): void {
    const dialogRef = this.dialog.open(ConfigurationDialogComponent, {
      data: <DialogData>{
        instructorName: this.clientIdentifier,
      },
    });

    dialogRef
      .afterClosed()
      .subscribe(result => this.handleDialogResult(result));
  }

  private async handleDialogResult(result: any) {
    if (result?.instructorName?.length) this.updateInstructor(result);

    if (!this.clientIdentifier) await this.showNoInstructorWarning();
  }

  private updateInstructor(result: any): void {
    this.clientIdentifier = result.instructorName;
    this.cookieService.setCookie(COOKIE_NAME, result.instructorName, 365);
    this.instructionService.subscribeToStream();
  }

  private async showNoInstructorWarning(): Promise<void> {
    const snackbarTexts = await this.translate
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
}

