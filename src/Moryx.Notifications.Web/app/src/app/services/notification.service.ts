/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { NotificationModel } from '@api/models';
import { NotificationPublisherService } from '@api/services';
import { ConnectionState } from '../models/ConnectionState';
import { HttpErrorResponse } from '@angular/common/http';
import { SnackbarService } from '@moryx/ngx-web-framework/services';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private readonly notificationPublisherService = inject(NotificationPublisherService);
  private readonly snackbarService = inject(SnackbarService);

  private eventSource?: EventSource;
  private readonly _notifications = signal<NotificationModel[]>([]);
  readonly notifications = this._notifications.asReadonly();
  private readonly _selection = signal<string | undefined>(undefined);
  readonly selection = this._selection.asReadonly();
  private readonly _state = signal<ConnectionState>(ConnectionState.Initializing);
  readonly state = this._state.asReadonly();

  connect() {
    this.eventSource = new EventSource(this.notificationPublisherService.rootUrl + '/api/moryx/notifications/stream');
    this.eventSource.onmessage = (event) => this.processNotifications(event);
    this.eventSource.onerror = (error) => this.processError(error);
  }

  private processNotifications(event: MessageEvent): void {
    const data: NotificationModel[] = JSON.parse(event.data);
    const notifications = data.filter(n => !!n.identifier).sortBySeverity();

    if (this.state() != ConnectionState.Connected) {
      this._state.set(ConnectionState.Connected);
    }
    this._notifications.set(notifications);
    this.checkSelection();
  }

  private processError(event: Event): void {
    this._state.set(ConnectionState.Reconnecting);
  }

  public select(identifier: string | undefined): void {
    let selected: string | undefined;
    const currentNotifications = this.notifications();

    if (!currentNotifications.length) {
      selected = undefined;
    }
    else if (currentNotifications.some(m => m.identifier === identifier)) {
      selected = identifier;
    }
    else {
      selected = currentNotifications[0].identifier;
    }

    this._selection.set(selected);
  }

  public get(identifier: string | undefined): NotificationModel | undefined {
    return this.notifications().find(n => n.identifier === identifier);
  }

  public acknowledge(identifier: string | undefined): void {
    if (!identifier) {
      return;
    }

    this.notificationPublisherService.acknowledge$Response({guid: identifier})
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  private checkSelection(): void {
    const currentSelection = this.selection();
    const requiresReset = !this.notifications().some(n => n.identifier === currentSelection);
    if (requiresReset) {
      this.resetSelection();
    }
  }

  private resetSelection() {
    this.select(undefined);
  }

  disconnect() {
    if (this.eventSource) {
      this.eventSource.close();
      this.eventSource = undefined;
    }
  }
}

