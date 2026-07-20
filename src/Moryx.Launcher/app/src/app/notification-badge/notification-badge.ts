/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, DestroyRef, effect, inject, input, signal, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-notification-badge',
  imports: [],
  templateUrl: './notification-badge.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './notification-badge.scss',
  host: {
    '(window:beforeunload)': 'closeEventSource()'
  }
})
export class NotificationBadge {
  private destroyRef = inject(DestroyRef);
  readonly eventStream = input('');
  protected count = signal(0);
  private eventSource: EventSource | undefined;

  constructor() {
    effect((onCleanup) => {
      const url = this.eventStream();
      if (!url) {
        return;
      }

      this.eventSource = new EventSource(url);
      this.eventSource.onmessage = (e) => this.onReceived(e);

      onCleanup(() => {
        this.closeEventSource();
      })
    });

    this.destroyRef.onDestroy(() => this.closeEventSource());
  }

  protected closeEventSource(): void {
    this.eventSource?.close();
    this.eventSource = undefined;
  }

  private onReceived(event: MessageEvent<string>) {
    // Check if data is plain number
    let integer = parseInt(event.data);
    // Parse data assuming collection
    if (!integer) {
      const list = JSON.parse(event.data);
      integer = list.length;
    }

    this.count.set(integer);
  }

  protected countString() {
    if (this.count() > 9) {
      return '9+';
    }
    return this.count();
  }
}

