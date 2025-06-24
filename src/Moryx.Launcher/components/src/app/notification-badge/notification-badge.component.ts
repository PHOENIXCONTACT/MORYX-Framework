import { Component, Input, input, NgZone, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-notification-badge',
  standalone: true,
  imports: [],
  templateUrl: './notification-badge.component.html',
  styleUrl: './notification-badge.component.css',
})
export class NotificationBadgeComponent implements OnInit, OnDestroy {
  @Input() eventstream: string = '';
  count: number = 0;
  eventSource: EventSource | undefined;

  constructor(private ngZone: NgZone) {}

  ngOnInit(): void {
    if (!this.eventstream) return;

    this.eventSource = new EventSource(this.eventstream);
    this.eventSource.onmessage = this.onReceived.bind(this);
  }

  ngOnDestroy(): void {
    this.eventSource?.removeEventListener('message',this.onReceived);
  }

  onReceived(event: any) {
    this.ngZone.run(() => {
      // Check if data is plain number
      var integer = parseInt(event.data);
      // Parse data assuming collection
      if (!integer) {
        const list = JSON.parse(event.data);
        integer = list.length;
      }

      this.count = integer;
    });
  }

  countString(){
    if(this.count > 9) return '9+';
    return this.count;
  }
}
