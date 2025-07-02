import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { Severity } from '../../api/models';
import { NotificationModel } from '../../api/models/notification-model';
import { NotificationService } from 'src/app/services/notification.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { getIcon } from 'src/app/utils';

@Component({
    selector: 'moryx-notifications',
    templateUrl: './notifications.component.html',
    styleUrls: ['./notifications.component.scss'],
    imports: [
      CommonModule,
      MatCardModule,
      MatIconModule
    ],
    standalone: true
})
export class NotificationsComponent implements OnInit, OnDestroy {
  notificationList = signal<NotificationModel[]>([]);
  hoveredNotificationIdentifier = signal<string | undefined>(undefined);
  selectedNotificationIdentifier = signal<string | undefined>(undefined);

  getIcon = getIcon;

  private notificationSubscription: Subscription|undefined;
  private selectionSubscription: Subscription|undefined;;

  constructor(private notificationService: NotificationService) {}
    
  ngOnInit(): void {
    this.notificationSubscription = this.notificationService.notifications$.subscribe(notifications => {
      this.notificationList.update(_ => notifications);
    });
    this.selectionSubscription = this.notificationService.selection$.subscribe(identifier => {
      this.selectedNotificationIdentifier.update(_ => identifier);
    });
  }

  onUpdateHoveredIdentifier(identifier: string | undefined){
    this.hoveredNotificationIdentifier.update(_ => identifier)
  }

  ngOnDestroy(): void {
    this.notificationSubscription?.unsubscribe();
    this.selectionSubscription?.unsubscribe();
  }

  select(notification: NotificationModel): void {
    this.notificationService.select(notification.identifier);
  }
}
