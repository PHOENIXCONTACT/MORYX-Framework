/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, effect, inject, input, OnInit, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { LogLevel, OperationLogMessageModel } from '@api/models';
import { OrderManagementService } from '@api/services';
import { TranslationConstants } from '@app/translation-constants';

@Component({
  selector: 'app-log-message-list',
  templateUrl: './log-message-list.html',
  styleUrls: ['./log-message-list.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    CommonModule,
    MatExpansionModule,
    TranslatePipe
  ]
})
export class LogMessageList implements OnInit {
  private translateService = inject(TranslateService);
  private orderManagementService = inject(OrderManagementService);

  readonly guid = input.required<string>();
  protected logMessages = signal<OperationLogMessageModel[]>([]);
  protected isLoading = signal<boolean>(false);
  protected notification = signal<string>('');

  protected TranslationConstants = TranslationConstants;
  translations: Record<string, string> = {};

  constructor() {
    effect(() => {
      const guid = this.guid();
      untracked(() => {
        this.fetchMessages(guid);
      });
    });
  }

  async ngOnInit() {
    this.translations = await firstValueFrom(this.translateService.get([TranslationConstants.OPERATIONS.EMPTY_LOG]));
  }

  private fetchMessages(guid: string) {
    this.isLoading.set(true);
    this.orderManagementService
      .getLogs({guid: guid})
      .then((logs: OperationLogMessageModel[]) => {
        this.notification.set(
          logs.length > 0 ? '' : this.translations[TranslationConstants.OPERATIONS.EMPTY_LOG]
        );
        this.logMessages.set(logs.sort((a, b) => this.sortDescending(a.timeStamp!, b.timeStamp!)));
        this.isLoading.set(false);
      })
      .catch((err: HttpErrorResponse) => {
        this.notification.set(err.message);
        this.isLoading.set(false);
      });
  }

  sortDescending(a: string, b: string): number {
    return a > b ? -1 : a < b ? 1 : 0;
  }

  protected getColor(logLevel?: LogLevel): string {
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

