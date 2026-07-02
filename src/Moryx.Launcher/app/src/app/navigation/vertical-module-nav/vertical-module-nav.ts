/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { NotificationBadge } from '../../notification-badge/notification-badge';
import { TranslatePipe } from '@ngx-translate/core';
import { ModuleNavBase } from '../module-nav-base';

@Component({
  selector: 'app-vertical-module-nav',
  imports: [MatListModule, MatIconModule, TranslatePipe, NotificationBadge],
  templateUrl: './vertical-module-nav.html',
  styleUrl: './vertical-module-nav.scss',
  host: {
    '[class.collapsed]': 'collapsed()',
  }
})
export class VerticalModuleNav extends ModuleNavBase {
  readonly collapsed = input(false);
}
