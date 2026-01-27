/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from '@angular/common';
import { AfterContentInit, Component, Input, NgZone, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-notifications',
  imports: [CommonModule],
  providers: [],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.css'
})
export class NotificationsComponent implements OnInit, OnDestroy {

  @Input() url: string = 'notifications';
  @Input() api: string = '/api/moryx/notifications/stream';
  notifications: Array<Notification> = [];
  eventSource: EventSource | undefined;

  constructor(private ngZone: NgZone) {
  }

  ngOnDestroy(): void {
    this.eventSource?.removeEventListener('message', this.onMessageReceived);
  }

  ngOnInit(): void {
    if (!this.api) return;
    // listen to notification stream
    this.eventSource = new EventSource(this.api);
    this.eventSource.onmessage = this.onMessageReceived.bind(this);
  }

  private onMessageReceived(event: any) {
    this.ngZone.run(() => {
      //send notifications to listeners
      const datas = <Array<Notification>>JSON.parse(event.data);
      this.notifications = datas;
    });
  }

  getNotificationToDisplay() {
    if (!this.notifications.length) return undefined;
    const highSeverityNotifications = this.getHighSeverityNotifications(this.notifications);
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
