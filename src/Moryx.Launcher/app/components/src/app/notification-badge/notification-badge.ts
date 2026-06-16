/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, HostListener, input, OnDestroy, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-notification-badge',
  imports: [],
  templateUrl: './notification-badge.html',
  styleUrl: './notification-badge.css'
})
export class NotificationBadge implements OnInit, OnDestroy {
  eventstream = input(''); // Do fix naming here, must be lower-case for HTML attribute
  count = signal(0);
  eventSource: EventSource | null = null;

  ngOnInit(): void {
    if (!this.eventstream()) {
      return;
    }

    this.eventSource = new EventSource(this.eventstream());
    this.eventSource.onmessage = this.onReceived.bind(this);
  }

  private clearAll() {
    if (this.eventSource) {
      this.eventSource.removeEventListener('message', this.onReceived);

      this.eventSource.close();
      this.eventSource = null;
    }
  }

  ngOnDestroy(): void {
    this.clearAll();
  }

  @HostListener('window:beforeunload')
  onBeforeUnload() {
    this.clearAll();
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

