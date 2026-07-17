/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnDestroy, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { LanguageService, SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { filter, firstValueFrom, Subscription } from 'rxjs';
import { environment } from '../environments/environment';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { SessionsService } from './services/sessions.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { Toolbox } from './components/toolbox/toolbox';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatSidenavModule,
    MatToolbarModule,
    MatTooltipModule,
    MatIconModule,
    TranslatePipe,
    Toolbox,
    RouterModule,
    MatButtonModule
  ]
})
export class App implements OnInit, OnDestroy {
  protected router = inject(Router);
  private sessionService = inject(SessionsService);
  private languageService = inject(LanguageService);
  private snackbarService = inject(SnackbarService);
  private translateService = inject(TranslateService);

  protected title = 'Workplan Editor';
  protected readonly workplansToolbarImage = environment.assets + 'assets/workplans-toolbar.jpg';

  protected changeViewDisabled = signal(true);
  protected navigatedUrl = signal('');
  protected changeViewTooltip = signal('');

  private subscriptions: Subscription[] = [];

  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translateService.setFallbackLang('en');
    this.translateService.use(this.languageService.getFallbackLang());
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await firstValueFrom(this.translateService
      .get([
        TranslationConstants.APP.OPEN_WORKPLAN_MANAGEMENT,
        TranslationConstants.APP.OPEN_WORKPLAN_SESSIONS,
        TranslationConstants.APP.NO_SESSION_IS_OPEN,
      ]));
  }

  ngOnInit(): void {
    const routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(async (e: NavigationEnd) => {
        const translations = await this.getTranslations();
        this.navigatedUrl.set(e.url);
        if (this.navigatedUrl() !== '/management') {
          this.changeViewTooltip.set(translations[TranslationConstants.APP.OPEN_WORKPLAN_MANAGEMENT]);
          this.changeViewDisabled.set(false);
          return;
        }

        if (this.sessionService.activeSession()) {
          this.changeViewTooltip.set(translations[TranslationConstants.APP.OPEN_WORKPLAN_SESSIONS]);
          this.changeViewDisabled.set(false);
          return;
        }

        this.changeViewTooltip.set(translations[TranslationConstants.APP.NO_SESSION_IS_OPEN]);
        this.changeViewDisabled.set(true);
      });

    this.subscriptions.push(routerSubscription);
  }

  protected changeView() {
    if (this.navigatedUrl() !== '/management') {
      this.router.navigate(['/management']);
    }
    else if (this.sessionService.activeSession()) {
      this.router.navigate(['session', this.sessionService.activeSession()]);
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  protected onAdd() {
    this.sessionService
      .getSessionForWorkplan(0)
      .then(session => {
        this.router.navigate(['session', session.sessionToken]);
        this.sessionService.activateSession(session.sessionToken!);
      })
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
  }
}

