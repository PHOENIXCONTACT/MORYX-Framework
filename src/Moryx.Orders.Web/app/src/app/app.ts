/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './translation-constants';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    RouterOutlet
  ]
})
export class App implements OnInit {
  private translateService = inject(TranslateService);

  title = 'Orders';
  TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.translateService.get([TranslationConstants.APP.TITLE]).subscribe(title => {
      this.title = title;
    });
  }
}

