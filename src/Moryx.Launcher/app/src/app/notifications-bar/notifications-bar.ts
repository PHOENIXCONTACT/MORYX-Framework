/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  DestroyRef,
  effect,
  ElementRef,
  inject,
  input,
  signal,
  ChangeDetectionStrategy
} from '@angular/core';
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
  changeDetection: ChangeDetectionStrategy.Eager,
  host: {
    '[style.--severity-background-color]': 'getSeverityBackgroundColor(currentNotification()?.severity, errorAvailable())',
    '(window:beforeunload)': 'clearAll()'
  }
})
export class NotificationsBar {
  private destroyRef = inject(DestroyRef);
  private elementRef = inject(ElementRef);

  readonly url = input('Notifications');
  readonly api = input('/api/moryx/notifications/stream');

  private eventSource: EventSource | undefined;

  protected notifications = signal<Array<Notification> | undefined>(undefined);
  protected notificationIndex = signal<number>(0);
  protected errorAvailable = signal<boolean>(false);

  protected currentNotification = computed<Notification | undefined | null>(() => {
    const notifications = this.notifications();

    if (notifications === undefined) {
      return undefined;
    }

    if (notifications.length === 0) {
      return null;
    }

    return notifications[this.notificationIndex()];
  });

  private intervalId: number | undefined = undefined;

  private severityRank: Record<Severity, number> = {
    Info: 0,
    Warning: 1,
    Error: 2,
    Fatal: 3
  };

  constructor() {
    effect((onCleanup) => {
      const url = this.api();
      if (!url) {
        return;
      }

      this.eventSource = new EventSource(url);
      this.eventSource.onmessage = (e) => this.onMessageReceived(e);
      this.eventSource.onerror = (e) => this.onErrorReceived(e);

      onCleanup(() => {
        this.clearAll();
      });
    });

    this.destroyRef.onDestroy(() => {
      this.clearAll()
    });
  }

  protected clearAll() {
    this.clearEventSource();
    this.clearInterval();
  }

  private onMessageReceived(event: MessageEvent<string>) {
    //send notifications to listeners
    const data = <Array<Notification>>JSON.parse(event.data);

    // Reverse to get the most recent first and stable sort by severity descending, which results in highest severity first.
    data.reverse().sort((first, second) => {
      return this.severityRank[second.severity] - this.severityRank[first.severity]
    });

    if (data.length > 0) {
      // Reduce to the highest severity only
      const firstSeverity = data[0].severity;
      const dataSplit = data.findIndex(x => x.severity !== firstSeverity);
      const reducedData = dataSplit === -1 ? data : data.slice(0, dataSplit);

      this.notificationIndex.set(0);
      this.notifications.set(reducedData);

      this.updateInterval();
    } else {
      this.notificationIndex.set(0);
      this.notifications.set([]);

      this.clearInterval();
    }

    this.errorAvailable.set(false);
  }

  private onErrorReceived(event: Event) {
    if (!this.errorAvailable()) {
      this.errorAvailable.set(true);

      this.notificationIndex.set(0);
      this.notifications.set([]);
      this.clearInterval();
    }
  }

  private clearEventSource() {
    if (this.eventSource !== undefined) {
      this.eventSource.close();
      this.eventSource = undefined;
    }
  }

  private updateInterval() {
    this.clearInterval();

    this.intervalId = window.setInterval(() => {
      const notifications = this.notifications();

      if (notifications === undefined) {
        return;
      }

      this.notificationIndex.update(v => (v + 1) % notifications.length);
    }, 5000);
  }

  private clearInterval() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = undefined;
    }
  }

  protected getSeverityBackgroundColor(severity: Severity | undefined | null, errorAvailabe: boolean): string {
    const computedStyle = getComputedStyle(this.elementRef.nativeElement);

    if (errorAvailabe) {
      return computedStyle.getPropertyValue('--color-Info').trim();
    }

    if (severity === undefined || severity === null) {
      return computedStyle.getPropertyValue('--color-Success').trim();
    }

    const color = computedStyle.getPropertyValue('--color-' + severity).trim();

    return color;
  }

  protected getIcon(severity: Severity | undefined): string {
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
}
