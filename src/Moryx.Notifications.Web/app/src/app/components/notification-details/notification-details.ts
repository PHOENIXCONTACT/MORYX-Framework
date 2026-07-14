/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { NotificationService } from '@app/services/notification.service';
import { environment } from '../../../environments/environment';
import { NotificationModel } from '@api/models/notification-model';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { getIcon } from '@app/utils';
import { MarkdownComponent, MarkdownService } from 'ngx-markdown';
import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-notification-details',
  templateUrl: './notification-details.html',
  styleUrls: ['./notification-details.scss'],
  imports: [
    DatePipe,
    MatCardModule,
    MatIconModule,
    TranslatePipe,
    MarkdownComponent,
    NavigableEntryEditor,
    MatButtonModule
  ],
  changeDetection: ChangeDetectionStrategy.Eager,
  providers: [MarkdownService]
})
export class NotificationDetails {
  private notificationService = inject(NotificationService);

  protected notification = computed(() => {
    const identifier = this.notificationService.selection();
    return identifier ? this.notificationService.get(identifier) : undefined;
  });

  protected TranslationConstants = TranslationConstants;
  protected getIcon = getIcon;
  protected environment = environment;

  protected onAcknowledge(notification: NotificationModel): void {
    this.notificationService.acknowledge(notification.identifier);
  }

  protected isArrayNotEmpty(array: unknown[] | undefined | null): boolean {
    return array !== undefined && array !== null && array.length > 0;
  }
}

