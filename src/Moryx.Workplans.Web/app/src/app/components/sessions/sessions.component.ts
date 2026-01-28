/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { SnackbarService, SearchBarService, SearchRequest, SearchSuggestion } from '@moryx/ngx-web-framework/services';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SubscriptionLike } from 'rxjs';
import { WorkplanSessionModel } from '../../api/models';
import { WorkplanEditingService } from '../../api/services';
import {
  ConfirmDialogButton,
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../dialogs/dialog-confirm/dialog-confirm.component';
import { TranslationConstants } from '../../extensions/translation-constants.extensions';
import { SessionsService } from '../../services/sessions.service';
import { EditorStateService } from '../../services/editor-state.service';

import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-sessions',
  templateUrl: './sessions.component.html',
  styleUrls: ['./sessions.component.scss'],
  standalone: true,
  imports: [
    MatTabsModule,
    MatProgressSpinnerModule,
    RouterOutlet,
    MatIconModule,
    MatTooltipModule,
    FormsModule,
    TranslateModule,
    MatButtonModule
  ]
})
export class SessionsComponent implements OnInit, OnDestroy {
  constructor(
    private sessionService: SessionsService,
    private workplanEditing: WorkplanEditingService,
    private snackbarService: SnackbarService,
    private router: Router,
    public dialog: MatDialog,
    private searchBarService: SearchBarService,
    public translate: TranslateService,
    private editorState: EditorStateService
  ) {
  }

  sessions = signal<WorkplanSessionModel[]>([]);
  activeSession = signal<WorkplanSessionModel | undefined>(undefined);

  private subscriptions: SubscriptionLike[] = [];
  TranslationConstants = TranslationConstants;

  async ngOnInit(): Promise<void> {
    const availableSessionsSubscription = this.sessionService.availableSessions$.subscribe(
      async tokens => await this.onSessionsChanged(tokens)
    );
    this.subscriptions.push(availableSessionsSubscription);

    const activeSessionSubscription = this.sessionService.activeSession$.subscribe(
      async token => await this.onActiveSessionChanged(token)
    );
    this.subscriptions.push(activeSessionSubscription);

    const sessionUpdatedSubscription = this.sessionService.sessionUpdated$.subscribe(session =>
      this.onSessionUpdated(session)
    );
    this.subscriptions.push(sessionUpdatedSubscription);

    this.searchBarService.subscribe({
      next: (request: SearchRequest) => {
        this.onSearch(request);
      }
    });
  }

  onSessionUpdated(updated: WorkplanSessionModel) {
    if (this.activeSession()?.sessionToken === updated.sessionToken) this.activeSession.update(_ => updated);

    this.sessions.update(_ => this.sessions().filter(s => s.sessionToken !== updated.sessionToken));
    this.sessions.update(items => {
      items.push(updated);
      return items;
    });
  }

  private async onSessionsChanged(tokens: string[]): Promise<void> {
    const newSessions = <WorkplanSessionModel[]>[];
    await Promise.all(
      tokens.map(
        async token =>
          await this.sessionService
            .getSession(token)
            .toAsync()
            .then((value: WorkplanSessionModel) => newSessions.push(value))
            .catch(async (err: HttpErrorResponse) => await this.snackbarService.handleError(err))
      )
    ).then(() => (this.sessions.update(_ => newSessions)));
  }

  private async onActiveSessionChanged(token: string | undefined) {
    const result = token ? await this.sessionService.getSession(token).toAsync() : undefined;
    this.activeSession.update(_ => result);
    if (this.activeSession()) this.router.navigate(['session', this.activeSession()?.sessionToken]);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
    this.searchBarService.unsubscribe();
  }

  async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate
      .get([
        TranslationConstants.SESSIONS.CONFIRM_DIALOG.CONFIRM,
        TranslationConstants.SESSIONS.CONFIRM_DIALOG.MESSAGE,
        TranslationConstants.SESSIONS.CONFIRM_DIALOG.TITLE,
        TranslationConstants.SESSIONS.CONFIRM_DIALOG.CANCEL,
        TranslationConstants.EDITOR.SNACK_BAR.SUCCESS
      ])
      .toAsync();
  }

  private onSearch(request: SearchRequest) {
    if (!this.sessions().length) return;

    const urlWorkplans = 'Workplans/';
    const urlSession = 'session/';
    const searchterm = request.term.toLowerCase();
    let sessions = this.sessions().filter(s => s.name?.toLowerCase().includes(searchterm));

    if (!sessions) sessions = [];

    if (request.submitted) {
      this.searchBarService.clearSuggestions();

      if (sessions.length === 1 && sessions[0].sessionToken && sessions[0].name) {
        const session = sessions[0];
        this.activateSession(session.sessionToken!);
        this.router.navigate([urlSession + session.sessionToken]);
      }
      this.searchBarService.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        }
      });
    } else {
      const searchSuggestions = [] as SearchSuggestion[];
      for (let session of sessions) {
        if (!session.sessionToken || !session.name) continue;

        const url = urlWorkplans + urlSession + session.sessionToken;
        searchSuggestions.push({ text: session.name, url: url });
      }

      this.searchBarService.provideSuggestions(searchSuggestions);
    }
  }

  closeSession(sessionToken: string, sessionIndex: number) {
    this.sessionService.closeSession(sessionToken).subscribe({
      next: () => {
        if (sessionIndex > 0) {
          this.activateSession(this.sessions()[sessionIndex - 1].sessionToken!);
          this.router.navigate(['session', this.sessions()[sessionIndex - 1].sessionToken]);
        } else if (this.sessions().length > 1) {
          this.activateSession(this.sessions()[1].sessionToken!);
          this.router.navigate(['session', this.sessions()[1].sessionToken]);
        } else this.router.navigate(['management']);
      },
      error: async (err: HttpErrorResponse) => await this.snackbarService.handleError(err)
    });
  }

  activateSession(token: string): void {
    this.sessionService.activateSession(token);
  }

  async onCloseSession(sessionToken: string | undefined) {
    if (!sessionToken) return;

    const sessionIndex = this.sessions().findIndex(s => s.sessionToken === sessionToken);
    if (sessionIndex < 0) return;

    const translations = await this.getTranslations();

    const dialog = this.dialog.open(ConfirmDialogComponent, {
      autoFocus: false,
      data: <ConfirmDialogData>{
        title: translations[TranslationConstants.SESSIONS.CONFIRM_DIALOG.TITLE],
        message: translations[TranslationConstants.SESSIONS.CONFIRM_DIALOG.MESSAGE],
        buttons: [
          <ConfirmDialogButton>{
            text: translations[TranslationConstants.SESSIONS.CONFIRM_DIALOG.CANCEL],
            action: () => dialog.close()
          },
          <ConfirmDialogButton>{
            text: translations[TranslationConstants.SESSIONS.CONFIRM_DIALOG.CONFIRM],
            focused: true,
            action: () => {
              this.closeSession(sessionToken, sessionIndex);
              dialog.close();
            }
          }
        ]
      }
    });
  }

  isSessionActive(token: string): boolean {
    return this.activeSession()?.sessionToken === token;
  }

  saveWorkplan() {
    if (!this.activeSession()) return;

    const session = this.activeSession()!;
    this.sessionService.updateSession(session).toAsync()
      .catch(async (err: HttpErrorResponse) => await this.snackbarService.handleError(err))
      .then(_ => this.saveSession(session));
  }

  private saveSession(session: WorkplanSessionModel) {
    this.sessionService.saveSession(session).toAsync()
      .catch(async (err: HttpErrorResponse) => await this.snackbarService.handleError(err))
      .then(async session => {
        if (!session) return;
        this.editorState.setWorkplan(session);
        const translations = await this.getTranslations();
        this.snackbarService.showSuccess(translations[TranslationConstants.EDITOR.SNACK_BAR.SUCCESS]);
      });
  }

  autoLayout() {
    this.workplanEditing.autoLayout({ sessionId: this.activeSession()?.sessionToken! }).subscribe({
      next: layoutedSession => {
        this.sessionService.registerUpdatedSession(layoutedSession);
        this.editorState.setWorkplan(layoutedSession);
      },
      error: async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
    });
  }
}

