/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from '@angular/common';
import { Component, computed, ElementRef, input, OnDestroy, OnInit, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';


export type Severity = 'Info' | 'Warning' | 'Error' | 'Fatal';

export interface Notification {
  severity: Severity;
  title: string;
}

@Component({
  selector: 'app-notifications-bar',
  imports: [CommonModule, MatIconModule],
  providers: [],
  templateUrl: './notifications-bar.html',
  styleUrl: './notifications-bar.scss',
  host: {
    '[style.--severity-background-color]': 'getSeverityBackgroundColor(currentNotification()?.severity)'
  }
})
export class NotificationsBar implements OnInit, OnDestroy {
  url = input('notifications');
  api = input('/api/moryx/notifications/stream');
  eventSource: EventSource | undefined;

  notifications = signal<Array<Notification>|undefined>(undefined);
  notificationIndex = signal<number>(0);
  currentNotification = computed<Notification|undefined|null>(() => {
    const notifications = this.notifications();

    if (notifications === undefined)
      return undefined;

    if (notifications.length === 0)
      return null;

    return notifications[this.notificationIndex()];
  });

  private intervalId: number|undefined = undefined;

  private severityRank: Record<Severity, number> = {
    Info: 0,
    Warning: 1,
    Error: 2,
    Fatal: 3
  };

  constructor(private elementRef: ElementRef) {}

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
    const data = <Array<Notification>>JSON.parse(event.data);

    // Reverse to get the most recent first and stable sort by severity descending, which results in highest severity first.
    data.reverse().sort((first, second) => {
      return this.severityRank[second.severity] - this.severityRank[first.severity]
    });

    if (data.length > 0) {
      // Reduce to highest severity only
      const firstSeverity = data[0].severity;
      const dataSplit = data.findIndex(x => x.severity !== firstSeverity);
      const reducedData = dataSplit === -1 ? data : data.slice(0, dataSplit);

      this.notificationIndex.set(0);
      this.notifications.set(reducedData);

      this.updateInterval();
    }
    else {
      this.notificationIndex.set(0);
      this.notifications.set([]);

      this.clearInterval();
    }
  }

  updateInterval() {
    this.clearInterval();

    this.intervalId = window.setInterval(() => {
      const notifications = this.notifications();

      if (notifications === undefined)
        return;

      this.notificationIndex.update(v => (v + 1) % notifications.length);
    }, 5000);
  }

  clearInterval() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = undefined;
    }
  }

  getSeverityBackgroundColor(severity: Severity | undefined | null): string {
    const computedStyle = getComputedStyle(this.elementRef.nativeElement);

     if (severity === undefined || severity === null)
      return computedStyle.getPropertyValue('--color-Success').trim();;

    const color = computedStyle.getPropertyValue('--color-' + severity).trim();

    return color;
  }

  getIcon(severity: Severity | undefined): string {
    switch (severity) {
      case 'Info':
        return 'info_outline';
      case 'Warning':
        return 'warning_amber';
      case 'Error':
        return 'error_outline';
      case 'Fatal':
        return 'new_releases';
      default:
        return '';
    }
  }

  ngOnDestroy(): void {
    this.eventSource?.removeEventListener('message', this.onMessageReceived);
    this.clearInterval();
  }
}
