/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { WorkplanSessionModel } from '@api/models';
import { WorkplanEditingService } from '@api/services';
import { PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';
import { BrowserStorageService } from './browser-storage.service';

@Injectable({
  providedIn: 'root',
})
export class SessionsService {
  private workplanEditing = inject(WorkplanEditingService);
  private browserStorage = inject(BrowserStorageService);
  private cachedSessionModels = new Map<string, WorkplanSessionModel>();

  private readonly _activeSession = signal<string | undefined>(this.browserStorage.getActiveSession());
  readonly activeSession = this._activeSession.asReadonly();
  private readonly _availableSessions = signal<string[]>(
    this.browserStorage.getStorageSessions().map(sso => sso.sessionToken)
  );
  readonly availableSessions = this._availableSessions.asReadonly();
  private readonly _sessionUpdated = signal<WorkplanSessionModel | undefined>(undefined);
  readonly sessionUpdated = this._sessionUpdated.asReadonly();

  async getSession(sessionToken: string): Promise<WorkplanSessionModel> {
    const cachedModel = this.cachedSessionModels.get(sessionToken);
    if (cachedModel) {
      return cachedModel;
    }

    const session = await this.workplanEditing.openSession({ sessionId: sessionToken });
    this.processOpenedSession(session);
    return session;
  }

  async getSessionForWorkplan(workplanId: number, duplicate: boolean = false): Promise<WorkplanSessionModel> {
    let cachedModel = undefined;
    for (const cs of this.cachedSessionModels.values()) {if (cs.workplanId === workplanId) {
      cachedModel = cs;}
    }
    if (cachedModel) {
      return cachedModel;
    }

    const session = await this.workplanEditing.editWorkplan({ body: { workplanId: workplanId, duplicate: duplicate } });
    this.processOpenedSession(session);
    return session;
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

    this._availableSessions.update(sessions => [...sessions, session.sessionToken!]);

    this.addSessionToCache(session);
  }

  private addSessionToCache(session: WorkplanSessionModel) {
    this.cachedSessionModels.set(session.sessionToken!, session);
  }

  async saveSession(session: WorkplanSessionModel): Promise<WorkplanSessionModel> {
    session.nodes?.forEach(n => {
      if (n?.properties) {
        PrototypeToEntryConverter.convertToEntry(n?.properties);
      }
    });

    const saved = await this.workplanEditing.saveSession({ sessionId: session.sessionToken!, body: session });
    this.registerUpdatedSession(saved);
    return saved;
  }

  async updateSession(session: WorkplanSessionModel): Promise<WorkplanSessionModel> {
    const updated = await this.workplanEditing.updateSession({ sessionId: session.sessionToken!, body: session });
    this.registerUpdatedSession(updated);
    return updated;
  }

  registerUpdatedSession(session: WorkplanSessionModel) {
    this.cachedSessionModels.set(session.sessionToken!, session);
    this.browserStorage.updateSession(session);
    this._sessionUpdated.set(session);
  }

  async activateSession(sessionToken: string){
    if (!this.availableSessions().any(t => t === sessionToken)) {
      await this.getSession(sessionToken);
    }

    this.browserStorage.setActiveSession(sessionToken);
    this._activeSession.set(sessionToken);
  }

  deactivateSession() {
    this.browserStorage.removeActiveSession();
    this._activeSession.set(undefined);
  }

  async closeSession(sessionToken: string): Promise<void> {
    await this.workplanEditing.closeSession({ sessionId: sessionToken });
    this.processSessionClosed(sessionToken);
  }

  private processSessionClosed(sessionToken: string): void {
    this.browserStorage.closeSession(sessionToken);
    this.cachedSessionModels.delete(sessionToken);

    this._availableSessions.update(sessions => sessions.filter(st => st != sessionToken));

    if (this.activeSession() != sessionToken) {
      return;
    }

    this.browserStorage.removeActiveSession();
    this._activeSession.set(undefined);
  }
}

