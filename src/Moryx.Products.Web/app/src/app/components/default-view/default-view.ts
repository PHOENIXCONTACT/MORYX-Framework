/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, inject, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';

@Component({
  selector: 'app-default-view',
  templateUrl: './default-view.html',
  styleUrls: ['./default-view.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    EmptyState
  ]
})
export class DefaultView implements OnInit {
  protected headerText = signal('');
  protected messageText = signal('');

  private router = inject(Router);
  private translateService = inject(TranslateService);
  protected TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.getHeaderAndMessage();
  }

  private getHeaderAndMessage() {
    this.translateService
      .get([
        TranslationConstants.APP.EMPTY_STATE_HEADER,
        TranslationConstants.APP.EMPTY_STATE_TEXT,
        TranslationConstants.APP.EMPTY_STATE_RECIPES_HEADER,
        TranslationConstants.APP.EMPTY_STATE_RECIPES_TEXT,
        TranslationConstants.APP.EMPTY_STATE_PARTS_HEADER,
        TranslationConstants.APP.EMPTY_STATE_PARTS_TEXT,
      ])
      .subscribe(translations => {
        if (this.router.url.includes('recipes')) {
          this.headerText.set(translations[TranslationConstants.APP.EMPTY_STATE_RECIPES_HEADER]);
          this.messageText.set(translations[TranslationConstants.APP.EMPTY_STATE_RECIPES_TEXT]);
          return;
        }

        if (this.router.url.includes('parts')) {
          this.headerText.set(translations[TranslationConstants.APP.EMPTY_STATE_PARTS_HEADER]);
          this.messageText.set(translations[TranslationConstants.APP.EMPTY_STATE_PARTS_TEXT]);
          return;
        }

        this.headerText.set(translations[TranslationConstants.APP.EMPTY_STATE_HEADER]);
        this.messageText.set(translations[TranslationConstants.APP.EMPTY_STATE_TEXT]);
      });
  }
}

