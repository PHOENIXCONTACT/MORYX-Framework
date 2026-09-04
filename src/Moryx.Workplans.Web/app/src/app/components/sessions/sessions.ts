/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, effect, inject, OnDestroy, OnInit, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router, RouterOutlet } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { SnackbarService, SearchBarService, SearchRequest, SearchSuggestion } from '@moryx/ngx-web-framework/services';
import { PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { WorkplanSessionModel } from '@api/models';
import { WorkplanEditingService } from '@api/services';
import { ConfirmDialog, ConfirmDialogData } from '@app/dialogs/dialog-confirm/dialog-confirm';
import { TranslationConstants } from '@app/translation-constants';
import { SessionsService } from '@app/services/sessions.service';
import { EditorStateService } from '@app/services/editor-state.service';

import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-sessions',
  templateUrl: './sessions.html',
  styleUrls: ['./sessions.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatTabsModule,
    MatProgressSpinnerModule,
    RouterOutlet,
    MatIconModule,
    MatTooltipModule,
    FormsModule,
    TranslatePipe,
    MatButtonModule
  ]
})
export class Sessions implements OnInit, OnDestroy {
  private sessionService = inject(SessionsService);
  private workplanEditingService = inject(WorkplanEditingService);
  private snackbarService = inject(SnackbarService);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private searchBarService = inject(SearchBarService);
  private translateService = inject(TranslateService);
  private editorStateService = inject(EditorStateService);

  protected sessions = signal<WorkplanSessionModel[]>([]);
  protected activeSession = signal<WorkplanSessionModel | undefined>(undefined);

  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const tokens = this.sessionService.availableSessions();
      untracked(() => {
        this.onSessionsChanged(tokens);
      });
    });
    effect(() => {
      const token = this.sessionService.activeSession();
      untracked(() => {
        this.onActiveSessionChanged(token);
      });
    });
    effect(() => {
      const session = this.sessionService.sessionUpdated();
      if (session) {
        untracked(() => {
          this.onSessionUpdated(session);
        });
      }
    });
  }

  async ngOnInit(): Promise<void> {
    this.searchBarService.subscribe({
      next: (request: SearchRequest) => {
        this.onSearch(request);
      }
    });
  }

  private onSessionUpdated(updated: WorkplanSessionModel) {
    if (this.activeSession()?.sessionToken === updated.sessionToken) {
      this.activeSession.set(updated);
    }

    this.sessions.update(current => current.filter(s => s.sessionToken !== updated.sessionToken));
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
            .then((value: WorkplanSessionModel) => newSessions.push(value))
            .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err))
      )
    ).then(() => (this.sessions.set(newSessions)));
  }

  private async onActiveSessionChanged(token: string | undefined) {
    const result = token ? await this.sessionService.getSession(token) : undefined;
    this.activeSession.set(result);
    if (this.activeSession()) {
      this.router.navigate(['session', this.activeSession()?.sessionToken]);
    }
  }

  ngOnDestroy(): void {
    this.searchBarService.unsubscribe();
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await firstValueFrom(this.translateService
      .get([
        TranslationConstants.SESSIONS.CONFIRM_DIALOG.MESSAGE,
        TranslationConstants.SESSIONS.CONFIRM_DIALOG.TITLE,
        TranslationConstants.EDITOR.SNACK_BAR.SUCCESS
      ]));
  }

  private onSearch(request: SearchRequest) {
    if (!this.sessions().length) {
      return;
    }

    const urlWorkplans = 'Workplans/';
    const urlSession = 'session/';
    const searchterm = request.term.toLowerCase();
    let sessions = this.sessions().filter(s => s.name?.toLowerCase().includes(searchterm));

    if (!sessions) {
      sessions = [];
    }

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
      for (const session of sessions) {
        if (!session.sessionToken || !session.name) {
          continue;
        }

        const url = urlWorkplans + urlSession + session.sessionToken;
        searchSuggestions.push({text: session.name, url: url});
      }

      this.searchBarService.provideSuggestions(searchSuggestions);
    }
  }

  private closeSession(sessionToken: string, sessionIndex: number) {
    this.sessionService.closeSession(sessionToken)
      .then(() => {
        if (sessionIndex > 0) {
          this.activateSession(this.sessions()[sessionIndex - 1].sessionToken!);
          this.router.navigate(['session', this.sessions()[sessionIndex - 1].sessionToken]);
        } else if (this.sessions().length > 1) {
          this.activateSession(this.sessions()[1].sessionToken!);
          this.router.navigate(['session', this.sessions()[1].sessionToken]);
        } else {
          this.router.navigate(['management']);
        }
      })
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
  }

  protected activateSession(token: string): void {
    this.sessionService.activateSession(token);
  }

  protected async onCloseSession(sessionToken: string | undefined) {
    if (!sessionToken) {
      return;
    }

    const sessionIndex = this.sessions().findIndex(s => s.sessionToken === sessionToken);
    if (sessionIndex < 0) {
      return;
    }

    const translations = await this.getTranslations();

    const dialog = this.dialog.open(ConfirmDialog, {
      data: <ConfirmDialogData>{
        title: translations[TranslationConstants.SESSIONS.CONFIRM_DIALOG.TITLE],
        message: translations[TranslationConstants.SESSIONS.CONFIRM_DIALOG.MESSAGE],
      }
    });

    dialog.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.closeSession(sessionToken, sessionIndex);
      }
    });
  }

  protected isSessionActive(token: string): boolean {
    return this.activeSession()?.sessionToken === token;
  }

  protected async saveWorkplan() {
    if (!this.activeSession()) {
      return;
    }

    let session = this.activeSession()!;
    const editingNode = this.editorStateService.isEditingStep();

    if (editingNode?.id != null && session.sessionToken) {
      if (editingNode.properties) {
        PrototypeToEntryConverter.convertToEntry(editingNode.properties);
      }
      try {
        const flushedNode = await this.workplanEditingService
          .updateStep({ sessionId: session.sessionToken, nodeId: editingNode.id, body: editingNode });
        if (session.nodes) {
          session = { ...session, nodes: session.nodes.map(n => n.id === flushedNode.id ? flushedNode : n) };
        }
      } catch (err) {
        this.snackbarService.handleError(err as HttpErrorResponse);
        return;
      }
    }

    this.sessionService.updateSession(session)
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err))
      .then(_ => this.saveSession(session));
  }

  private saveSession(session: WorkplanSessionModel) {
    this.sessionService.saveSession(session)
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err))
      .then(async session => {
        if (!session) {
          return;
        }
        const editingNodeId = this.editorStateService.isEditingStep()?.id;
        this.editorStateService.setWorkplan(session);
        if (editingNodeId != null) {
          this.editorStateService.startEditingStep(editingNodeId);
        }
        const translations = await this.getTranslations();
        this.snackbarService.showSuccess(translations[TranslationConstants.EDITOR.SNACK_BAR.SUCCESS]);
      });
  }

  protected autoLayout() {
    this.workplanEditingService.autoLayout({sessionId: this.activeSession()?.sessionToken ?? ''})
      .then(layoutedSession => {
        this.sessionService.registerUpdatedSession(layoutedSession);
        this.editorStateService.setWorkplan(layoutedSession);
      })
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }
}

