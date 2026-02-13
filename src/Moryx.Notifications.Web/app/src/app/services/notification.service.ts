/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, NgZone, OnDestroy } from '@angular/core';
import { NotificationModel } from '../api/models';
import { NotificationPublisherService } from '../api/services';
import { BehaviorSubject, Observable } from 'rxjs';
import { ConnectionState } from '../models/ConnectionState';
import { HttpErrorResponse } from '@angular/common/http';
import { SnackbarService } from '@moryx/ngx-web-framework/services';

@Injectable({
  providedIn: 'root',
})
export class NotificationService implements OnDestroy {
  private eventSource: EventSource;
  private notificationSubject: BehaviorSubject<NotificationModel[]> = new BehaviorSubject<NotificationModel[]>([]);
  private selectionSubject: BehaviorSubject<string|undefined> = new BehaviorSubject<string|undefined>(undefined)
  private stateSubject: BehaviorSubject<ConnectionState> = new BehaviorSubject<ConnectionState>(ConnectionState.Initializing);

  public notifications$: Observable<NotificationModel[]> = this.notificationSubject.asObservable();
  public selection$: Observable<string|undefined> = this.selectionSubject.asObservable();
  public state$: Observable<ConnectionState> = this.stateSubject.asObservable();

  constructor(
    private zone: NgZone,
    private api: NotificationPublisherService,
    private snackbarService: SnackbarService) {

    this.eventSource = new EventSource(this.api.rootUrl + '/api/moryx/notifications/stream');
    this.eventSource.onmessage = (event) => this.processNotifications(event);
    this.eventSource.onerror = (error) => this.processError(error);
  }

  private processNotifications(event: MessageEvent) : void {
    const data: NotificationModel[] = JSON.parse(event.data);
    const notifications = data.filter(n => !!n.identifier).sortBySeverity();

    this.zone.run(() => {
      if (this.stateSubject.value != ConnectionState.Connected)
        this.stateSubject.next(ConnectionState.Connected)
      this.notificationSubject.next(notifications);
      this.checkSelection();
    });
  }

  private processError(event: Event) : void {
    this.zone.run(() => {
      this.stateSubject.next(ConnectionState.Reconnecting)
      this.notificationSubject.error(event);
    });
  }

  ngOnDestroy(): void {
    this.eventSource.close();
  }

  public select(identifier: string|undefined):void {
    let selected: string|undefined;
    const notifications = this.notificationSubject.value;

    if (!notifications.length)
      selected = undefined;
    else if(notifications.some(m => m.identifier === identifier))
      selected = identifier;
    else
      selected = notifications[0].identifier;

    this.selectionSubject.next(selected);
  }

  public get(identifier: string|undefined): NotificationModel|undefined {
    return this.notificationSubject.value.find(n => n.identifier === identifier);
  }

  public acknowledge(identifier: string|undefined): void {
    if(!identifier) return;

    this.api.acknowledge$Response({ guid: identifier }).subscribe({
      error: async (e: HttpErrorResponse) => await this.snackbarService.handleError(e),
    });
  }

  private checkSelection(): void {
    const currentSelection = this.selectionSubject.value
    const requiresReset = !this.notificationSubject.value.some(n => n.identifier === currentSelection)
    if(requiresReset)
      this.resetSelection();
  }

  private resetSelection() {
    this.select(undefined);
  }
}

