/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from '@angular/common';
import { Component, input, OnDestroy, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-notifications',
  imports: [CommonModule],
  providers: [],
  templateUrl: './notifications.html',
  styleUrl: './notifications.css'
})
export class Notifications implements OnInit, OnDestroy {
  url = input('notifications');
  api = input('/api/moryx/notifications/stream');
  notifications = signal<Array<Notification>>([]);
  eventSource: EventSource | undefined;

  ngOnDestroy(): void {
    this.eventSource?.removeEventListener('message', this.onMessageReceived);
  }

  ngOnInit(): void {
    if (!this.api()) {
      return;
    }

    // listen to notification stream
    this.eventSource = new EventSource(this.api());
    this.eventSource.onmessage = this.onMessageReceived.bind(this);
  }

  private onMessageReceived(event: any) {
    //send notifications to listeners
    const datas = <Array<Notification>>JSON.parse(event.data);
    this.notifications.set(datas);
  }

  getNotificationToDisplay() {
    if (!this.notifications().length) {
      return undefined;
    }
    
    const highSeverityNotifications = this.getHighSeverityNotifications(this.notifications());
    const latestNotifications = highSeverityNotifications.reverse(); // Descending order latest to oldest
    const latestNotification = latestNotifications[0];
    return latestNotification;
  }

  getSeverityBackgroundColor(severity: Severity | undefined) {
    switch (severity) {
      case 'Info':
        return '#808080';
      case 'Warning':
        return '#ebad34';
      case 'Error':
        return '#e9545d';
      case 'Fatal':
        return '#800080';
      default:
        return '#abcb3d';
    }
  }

  filterBySeverity(notifications: Array<Notification>, severity: Severity) {
    return notifications.filter(notification => notification.severity == severity);
  }


  getHighSeverityNotifications(notifications: Array<Notification>) {
    const fatals = this.filterBySeverity(notifications, 'Fatal');
    if (fatals.length) return fatals;

    const errors = this.filterBySeverity(notifications, 'Error');
    if (errors.length) return errors;

    const warnings = this.filterBySeverity(notifications, 'Warning');
    if (warnings.length) return warnings;

    const infos = this.filterBySeverity(notifications, 'Info');
    if (infos.length) return infos;

    return [];
  }

}

export interface Notification {
  severity: Severity;
  title: string;
}

export type Severity = 'Info' | 'Warning' | 'Error' | 'Fatal';
