/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnDestroy, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { NotificationService } from '@app/services/notification.service';
import { environment } from '../../../environments/environment';
import { NotificationModel } from '@api/models/notification-model';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { getIcon } from '@app/utils';
import { MarkdownComponent, MarkdownService } from 'ngx-markdown';
import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'notification-details',
  templateUrl: './notification-details.html',
  styleUrls: ['./notification-details.scss'],
  imports: [
    CommonModule,
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
export class NotificationDetails implements OnInit, OnDestroy {
  private notificationService = inject(NotificationService);

  protected notification = signal<NotificationModel | undefined>(undefined);

  subscription: Subscription | undefined;
  protected TranslationConstants = TranslationConstants;
  protected getIcon = getIcon;
  protected environment = environment;

  ngOnInit(): void {
    this.subscription = this.notificationService.selection$.subscribe(
      identifier => this.notification.update(_ => this.notificationService.get(identifier))
    );
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  protected onAcknowledge(notification: NotificationModel): void {
    this.notificationService.acknowledge(notification.identifier);
  }

  protected isArrayNotEmpty(array: any[] | undefined | null): boolean {
    return array !== undefined && array !== null && array.length > 0;
  }
}

