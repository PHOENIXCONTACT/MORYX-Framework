/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ChangeDetectionStrategy } from '@angular/core';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-variant-default-view',
  templateUrl: './variant-default-view.html',
  styleUrls: ['./variant-default-view.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [EmptyState, TranslatePipe]
})
export class VariantDefaultView {
  protected TranslationConstants = TranslationConstants;
}
