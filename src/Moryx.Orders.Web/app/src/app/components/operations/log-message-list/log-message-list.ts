/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from '@angular/common';
import { Component, effect, input, Input, OnInit, signal, untracked } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { delay, tap } from 'rxjs';
import { LogLevel } from '../../../api/models';
import { OperationLogMessageModel } from '../../../api/models';
import { OrderManagementService } from 'src/app/api/services';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';

@Component({
  selector: 'app-log-message-list',
  templateUrl: './log-message-list.html',
  styleUrls: ['./log-message-list.scss'],
  imports: [
    CommonModule,
    MatExpansionModule,
    TranslateModule,
  ],
  standalone: true
})
export class LogMessageList implements OnInit {
  guid = input.required<string>();
  logMessages = signal<OperationLogMessageModel[]>([]);
  isLoading = signal<boolean>(false);
  notification = signal<string>('');
  TranslationConstants = TranslationConstants;
  translations: Record<string, string> = {};

  constructor(public translate: TranslateService, private orderManagementService: OrderManagementService) {
    effect(() => {
      const guid = this.guid();
      untracked(() => this.fetchMessages(guid));
    });
  }

  async ngOnInit() {
    this.translations = await this.translate.get([TranslationConstants.OPERATIONS.EMPTY_LOG]).toAsync();
  }

  private fetchMessages(guid: string) {
    this.isLoading.update(_ => true);
    this.orderManagementService
      .getLogs({ guid: guid })
      .pipe(
        delay(1),
        tap(messages => {
          this.notification.update(_ =>
            messages.length > 0 ? '' : this.translations[TranslationConstants.OPERATIONS.EMPTY_LOG]
          );
        })
      )
      .subscribe({
        next: (logs: OperationLogMessageModel[]) => {
          this.logMessages.update(_ => logs.sort((a, b) => this.sortDescending(a.timeStamp!, b.timeStamp!)));
          this.isLoading.update(_ => false);
        },
        error: (err: any) => {
          this.notification.update(_ => err.message);
          this.isLoading.update(_ => false);
        },
      });
  }

  sortDescending(a: string, b: string): number {
    return a > b ? -1 : a < b ? 1 : 0;
  }

  getColor(logLevel?: LogLevel): string {
    switch (logLevel) {
      case LogLevel.Information:
        return 'chip-color-info';
      case LogLevel.Debug:
        return 'chip-color-debug';
      case LogLevel.Trace:
        return 'chip-color-trace';
      case LogLevel.Warning:
        return 'chip-color-warning';
      case LogLevel.Error:
        return 'chip-color-error';
      case LogLevel.Critical:
        return 'chip-color-fatal';
      default:
        return 'chip-color-info';
    }
  }
}

