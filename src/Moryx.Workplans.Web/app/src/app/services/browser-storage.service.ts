/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { WorkplanSessionModel } from '../api/models';

@Injectable({
  providedIn: 'root',
})
export class BrowserStorageService {
  private readonly SESSION_OBJECTS: string = 'sessions-objects';
  private readonly ACTIVE_SESSION: string = 'active-session';

  constructor() {}

  //#region ACTIVE_SESSION CRUD actions
  setActiveSession(sessionToken: string | undefined | null) {
    if (sessionToken) {
      sessionStorage.setItem(this.ACTIVE_SESSION, sessionToken);
    }
  }

  getActiveSession(): string | undefined {
    return sessionStorage.getItem(this.ACTIVE_SESSION) ?? undefined;
  }

  removeActiveSession(): void {
    sessionStorage.removeItem(this.ACTIVE_SESSION);
  }
  //#endregion

  //#region SESSIONS_OBJECTS CRUD actions
  setSessions(sessionStorageObject: SessionStorageObject[]): void {
    sessionStorage.setItem(this.SESSION_OBJECTS, JSON.stringify(sessionStorageObject));
  }

  getStorageSessions(): SessionStorageObject[] {
    const sessionStorageObjects = sessionStorage.getItem(this.SESSION_OBJECTS);
    return sessionStorageObjects?.length ? JSON.parse(sessionStorageObjects) : [];
  }

  addSession(session: WorkplanSessionModel) {
    const newStorageObject = this.toSessionStorageObject(session);

    const sessionStorageObjects = this.getStorageSessions();
    sessionStorageObjects.push(newStorageObject);
    this.setSessions(sessionStorageObjects);
  }

  updateSession(session: WorkplanSessionModel) {
    const updatedStorageObject = this.toSessionStorageObject(session);

    let currentStorageObjects = this.getStorageSessions();
    if (
      !currentStorageObjects.any(
        sso =>
          sso.sessionToken === updatedStorageObject.sessionToken &&
          (sso.name != updatedStorageObject.name || sso.workplanId != updatedStorageObject.workplanId)
      )
    )
      return;

    currentStorageObjects = currentStorageObjects.filter(sso => sso.sessionToken !== updatedStorageObject.sessionToken);
    currentStorageObjects.push(updatedStorageObject);
    this.setSessions(currentStorageObjects);
  }

  closeSession(sessionToken: string) {
    let sessionStorageObjects = this.getStorageSessions();
    sessionStorageObjects = sessionStorageObjects.filter(sso => sso.sessionToken !== sessionToken);
    this.setSessions(sessionStorageObjects);
    if (this.getActiveSession() === sessionToken) this.removeActiveSession();
  }
  //#endregion

  private toSessionStorageObject(session: WorkplanSessionModel): SessionStorageObject {
    return <SessionStorageObject>{
      sessionToken: session.sessionToken,
      name: session.name,
      workplanId: session.workplanId,
    };
  }
}

export interface SessionStorageObject {
  name: string;
  sessionToken: string;
  workplanId: number;
}

