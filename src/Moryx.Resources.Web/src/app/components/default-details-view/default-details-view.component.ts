/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit } from '@angular/core';
import { EmptyStateComponent } from '@moryx/ngx-web-framework';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-default-details-view',
    templateUrl: './default-details-view.component.html',
    styleUrls: ['./default-details-view.component.scss'],
    imports: [TranslateModule, EmptyStateComponent, ],
    standalone: true
})
export class DefaultDetailsViewComponent {
  TranslationConstants = TranslationConstants;
  
  constructor() {}
}

