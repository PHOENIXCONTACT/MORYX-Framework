/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component } from '@angular/core';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';

@Component({
  selector: 'app-default-details-view',
  templateUrl: './default-details-view.component.html',
  styleUrls: ['./default-details-view.component.scss'],
  imports: [TranslateModule, EmptyState,],
  standalone: true
})
export class DefaultDetailsViewComponent {
  TranslationConstants = TranslationConstants;

  constructor() {
  }
}

