/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { catchError, from, Observable, tap, throwError } from 'rxjs';
import { WorkplanSessionModel } from '@api/models';
import { WorkplanEditingService } from '@api/services';
import { PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';
import { BrowserStorageService } from './browser-storage.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class SessionsService {
  private workplanEditing = inject(WorkplanEditingService);
  private browserStorage = inject(BrowserStorageService);
  private cachedSessionModels = new Map<string, WorkplanSessionModel>();

  readonly activeSession = signal<string | undefined>(this.browserStorage.getActiveSession());
  readonly availableSessions = signal<string[]>(
    this.browserStorage.getStorageSessions().map(sso => sso.sessionToken)
  );
  readonly sessionUpdated = signal<WorkplanSessionModel | undefined>(undefined);

  getSession(sessionToken: string): Observable<WorkplanSessionModel> {
    const cachedModel = this.cachedSessionModels.get(sessionToken);
    if (cachedModel) {
      return from([cachedModel]);
    }

    return this.workplanEditing.openSession({ sessionId: sessionToken }).pipe(
      tap(session => this.processOpenedSession(session)),
      catchError((error: HttpErrorResponse): Observable<WorkplanSessionModel> => throwError(() => error))
    );
  }

  getSessionForWorkplan(workplanId: number, duplicate: boolean = false): Observable<WorkplanSessionModel> {
    let cachedModel = undefined;
    for (const cs of this.cachedSessionModels.values()) {if (cs.workplanId === workplanId) {
      cachedModel = cs;}
    }
    if (cachedModel) {
      return from([cachedModel]);
    }

    return this.workplanEditing.editWorkplan({ body: { workplanId: workplanId, duplicate: duplicate } }).pipe(
      tap(session => this.processOpenedSession(session)),
      catchError((error: HttpErrorResponse): Observable<WorkplanSessionModel> => throwError(() => error))
    );
  }

  private processOpenedSession(session: WorkplanSessionModel): void {
    if (!this.availableSessions().any(token => token === session.sessionToken)) {
      this.addNewSession(session);
    } else {
      this.addSessionToCache(session);
    }
  }

  private addNewSession(session: WorkplanSessionModel) {
    this.browserStorage.addSession(session);

    this.availableSessions.update(sessions => [...sessions, session.sessionToken!]);

    this.addSessionToCache(session);
  }

  private addSessionToCache(session: WorkplanSessionModel) {
    this.cachedSessionModels.set(session.sessionToken!, session);
  }

  saveSession(session: WorkplanSessionModel): Observable<WorkplanSessionModel> {
    session.nodes?.forEach(n => {
      if (n?.properties) {
        PrototypeToEntryConverter.convertToEntry(n?.properties);
      }
    });

    return this.workplanEditing.saveSession({ sessionId: session.sessionToken!, body: session }).pipe(
      tap(session => this.registerUpdatedSession(session)),
      catchError((error: HttpErrorResponse): Observable<WorkplanSessionModel> => throwError(() => error))
    );
  }

  updateSession(session: WorkplanSessionModel): Observable<WorkplanSessionModel> {
    return this.workplanEditing.updateSession({ sessionId: session.sessionToken!, body: session }).pipe(
      tap(session => this.registerUpdatedSession(session)),
      catchError((error: HttpErrorResponse): Observable<WorkplanSessionModel> => throwError(() => error))
    );
  }

  registerUpdatedSession(session: WorkplanSessionModel) {
    this.cachedSessionModels.set(session.sessionToken!, session);
    this.browserStorage.updateSession(session);
    this.sessionUpdated.set(session);
  }

  async activateSession(sessionToken: string){
    if (!this.availableSessions().any(t => t === sessionToken)) {
      await this.getSession(sessionToken).toAsync();
    }

    this.browserStorage.setActiveSession(sessionToken);
    this.activeSession.set(sessionToken);
  }

  deactivateSession() {
    this.browserStorage.removeActiveSession();
    this.activeSession.set(undefined);
  }

  closeSession(sessionToken: string): Observable<void> {
    return this.workplanEditing.closeSession({ sessionId: sessionToken }).pipe(
      tap(() => this.processSessionClosed(sessionToken)),
      catchError((error: HttpErrorResponse): Observable<void> => throwError(() => error))
    );
  }

  private processSessionClosed(sessionToken: string): void {
    this.browserStorage.closeSession(sessionToken);
    this.cachedSessionModels.delete(sessionToken);

    this.availableSessions.update(sessions => sessions.filter(st => st != sessionToken));

    if (this.activeSession() != sessionToken) {
      return;
    }

    this.browserStorage.removeActiveSession();
    this.activeSession.set(undefined);
  }
}

