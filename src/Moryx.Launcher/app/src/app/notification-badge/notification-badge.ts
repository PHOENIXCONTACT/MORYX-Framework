/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, HostListener, input, OnDestroy, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-notification-badge',
  imports: [],
  templateUrl: './notification-badge.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './notification-badge.scss'
})
export class NotificationBadge implements OnInit, OnDestroy {
  eventStream = input('');
  count = signal(0);
  eventSource: EventSource | undefined;

  ngOnInit(): void {
    console.log(this.eventStream());
    if (!this.eventStream()) {
      return;
    }

    this.eventSource = new EventSource(this.eventStream());
    this.eventSource.onmessage = this.onReceived.bind(this);
  }

  @HostListener('window:unload')
  onUnload(): void {
    this.eventSource?.close();
  }

  ngOnDestroy(): void {
    this.eventSource?.removeEventListener('message', this.onReceived);
    this.eventSource?.close();
  }

  onReceived(event: any) {
    // Check if data is plain number
      let integer = parseInt(event.data);
      // Parse data assuming collection
    if (!integer) {
      const list = JSON.parse(event.data);
      integer = list.length;
    }

    this.count.set(integer);
  }

  countString() {
    if (this.count() > 9) return '9+';
    return this.count();
  }
}

