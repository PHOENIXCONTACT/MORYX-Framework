/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ChangeDetectionStrategy } from '@angular/core';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants';

@Component({
  selector: 'app-default-details-view',
  templateUrl: './default-details-view.html',
  styleUrls: ['./default-details-view.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [TranslatePipe, EmptyState,]
})
export class DefaultDetailsView {
  protected TranslationConstants = TranslationConstants;
}
