/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { LanguageService } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { filter, Subscription } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { SessionsService } from './services/sessions.service';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { ToolboxComponent } from './components/toolbox/toolbox.component';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: true,
    imports:[
      MatSidenavModule,
      MatToolbarModule,
      CommonModule,
      MatTooltipModule,
      MatIconModule,
      TranslateModule,
      ToolboxComponent,
      RouterModule,
      MatButtonModule
    ]
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Workplan Editor';
  readonly workplansToolbarImage = environment.assets + 'assets/workplans-toolbar.jpg';

  changeViewDisabled = signal(true);
  navigatedUrl = signal('');
  changeViewTooltip = signal('');

  private activeSession: string | undefined;
  private subscriptions: Subscription[] = [];

  TranslationConstants = TranslationConstants;

  constructor(
    public router: Router,
    private sessionService: SessionsService,
    private languageService: LanguageService,
    private snackbarService: SnackbarService,
    public translate: TranslateService
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate
      .get([
        TranslationConstants.APP.OPEN_WORKPLAN_MANAGEMENT,
        TranslationConstants.APP.OPEN_WORKPLAN_SESSIONS,
        TranslationConstants.APP.NO_SESSION_IS_OPEN,
      ])
      .toAsync();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.sessionService.activeSession$.subscribe(as => (this.activeSession = as)));

    const routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(async (e: any) => {
        const translations = await this.getTranslations();
        this.navigatedUrl.update(_=> e['url']);
        if (this.navigatedUrl() !== '/management') {
          this.changeViewTooltip.update(_=> translations[TranslationConstants.APP.OPEN_WORKPLAN_MANAGEMENT]);
          this.changeViewDisabled.update(_=> false);
          return;
        }

        if (this.activeSession) {
          this.changeViewTooltip.update(_=> translations[TranslationConstants.APP.OPEN_WORKPLAN_SESSIONS]);
          this.changeViewDisabled.update(_=> false);
          return;
        }

        this.changeViewTooltip.update(_=> translations[TranslationConstants.APP.NO_SESSION_IS_OPEN]);
        this.changeViewDisabled.update(_=> true);
      });

    this.subscriptions.push(routerSubscription);
  }

  changeView() {
    if (this.navigatedUrl() !== '/management') this.router.navigate(['/management']);
    else if (this.activeSession) this.router.navigate(['session', this.activeSession]);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  onAdd() {
    this.sessionService
      .getSessionForWorkplan(0)
      .toAsync()
      .then(session => {
        this.router.navigate(['session', session.sessionToken]);
        this.sessionService.activateSession(session.sessionToken!);
      })
      .catch(async (err: HttpErrorResponse) => await this.snackbarService.handleError(err));
  }
}

