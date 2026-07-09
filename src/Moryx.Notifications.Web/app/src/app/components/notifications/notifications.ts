/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NotificationModel } from '@api/models/notification-model';
import { NotificationService } from '@app/services/notification.service';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { getIcon } from '@app/utils';

@Component({
    selector: 'app-notifications',
    templateUrl: './notifications.html',
    styleUrls: ['./notifications.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
      DatePipe,
      MatCardModule,
      MatIconModule
    ]
})
export class Notifications {
  private notificationService = inject(NotificationService);

  protected notificationList = toSignal(this.notificationService.notifications$, { initialValue: [] as NotificationModel[] });
  protected hoveredNotificationIdentifier = signal<string | undefined>(undefined);
  protected selectedNotificationIdentifier = toSignal(this.notificationService.selection$);

  protected getIcon = getIcon;

  protected onUpdateHoveredIdentifier(identifier: string | undefined){
    this.hoveredNotificationIdentifier.update(_ => identifier)
  }

  protected select(notification: NotificationModel): void {
    this.notificationService.select(notification.identifier);
  }
}

