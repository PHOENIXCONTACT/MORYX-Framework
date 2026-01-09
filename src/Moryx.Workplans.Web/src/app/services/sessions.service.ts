import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject, catchError, from, map, tap, throwError } from 'rxjs';
import { Observable } from 'rxjs';
import { WorkplanSessionModel } from '../api/models';
import { WorkplanEditingService } from '../api/services';
import { PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';
import { BrowserStorageService } from './browser-storage.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class SessionsService {
  private activeSession: BehaviorSubject<string | undefined>;
  activeSession$: Observable<string | undefined>;
  private availableSessions: BehaviorSubject<string[]>;
  availableSessions$: Observable<string[]>;
  private sessionUpdated = new Subject<WorkplanSessionModel>();
  sessionUpdated$ = this.sessionUpdated.asObservable();

  private cachedSessionModels = new Map<string, WorkplanSessionModel>();

  constructor(private workplanEditing: WorkplanEditingService, private browserStorage: BrowserStorageService) {
    this.activeSession = new BehaviorSubject<string | undefined>(browserStorage.getActiveSession());
    this.activeSession$ = this.activeSession.asObservable();
    this.availableSessions = new BehaviorSubject<string[]>(
      browserStorage.getStorageSessions().map(sso => sso.sessionToken)
    );
    this.availableSessions$ = this.availableSessions.asObservable();
  }

  getSession(sessionToken: string): Observable<WorkplanSessionModel> {
    const cachedModel = this.cachedSessionModels.get(sessionToken);
    if (cachedModel) return from([cachedModel]);

    return this.workplanEditing.openSession({ sessionId: sessionToken }).pipe(
      tap(session => this.processOpenedSession(session)),
      catchError((error: HttpErrorResponse): Observable<WorkplanSessionModel> => throwError(() => error))
    );
  }

  getSessionForWorkplan(workplanId: number, duplicate: boolean = false): Observable<WorkplanSessionModel> {
    let cachedModel = undefined;
    for (let cs of this.cachedSessionModels.values()) if (cs.workplanId === workplanId) cachedModel = cs;
    if (cachedModel) return from([cachedModel]);

    return this.workplanEditing.editWorkplan({ body: { workplanId: workplanId, duplicate: duplicate } }).pipe(
      tap(session => this.processOpenedSession(session)),
      catchError((error: HttpErrorResponse): Observable<WorkplanSessionModel> => throwError(() => error))
    );
  }

  private processOpenedSession(session: WorkplanSessionModel): void {
    if (!this.availableSessions.value.any(token => token === session.sessionToken)) this.addNewSession(session);
    else this.addSessionToCache(session);
  }

  private addNewSession(session: WorkplanSessionModel) {
    this.browserStorage.addSession(session);

    const newAvailableSessions = this.availableSessions.value;
    newAvailableSessions.push(session.sessionToken!);
    this.availableSessions.next(newAvailableSessions);

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
    this.sessionUpdated.next(session);
  }

  async activateSession(sessionToken: string){
    if (!this.availableSessions.value.any(t => t === sessionToken)) {
      await this.getSession(sessionToken).toAsync();
    }
          
    this.browserStorage.setActiveSession(sessionToken);
    this.activeSession.next(sessionToken);
  }

  deactivateSession() {
    this.browserStorage.removeActiveSession();
    this.activeSession.next(undefined);
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

    const remainingSessions = this.availableSessions.value.filter(st => st != sessionToken);
    this.availableSessions.next(remainingSessions);

    if (this.activeSession.value != sessionToken) return;

    this.browserStorage.removeActiveSession();
    this.activeSession.next(undefined);
  }
}
