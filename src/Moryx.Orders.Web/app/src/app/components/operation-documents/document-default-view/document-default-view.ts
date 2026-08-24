/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ChangeDetectionStrategy } from '@angular/core';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { TranslationConstants } from '@app/translation-constants';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-document-default-view',
  templateUrl: './document-default-view.html',
  styleUrls: ['./document-default-view.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [EmptyState, TranslatePipe]
})
export class DocumentDefaultView {
  protected TranslationConstants = TranslationConstants;
}
