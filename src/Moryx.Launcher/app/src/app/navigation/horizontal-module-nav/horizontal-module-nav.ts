/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { afterNextRender, Component, computed, DestroyRef, ElementRef, inject, signal, viewChild } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { NotificationBadge } from '../../notification-badge/notification-badge';
import { MoreMenu } from '../../more-menu/more-menu';
import { ModuleGridMenu } from '../module-grid-menu/module-grid-menu';
import { TranslatePipe } from '@ngx-translate/core';
import { ModuleNavBase } from '../module-nav-base';

const MIN_ITEM_WIDTH = 112; // items shrink below this -> remove from the end

@Component({
  selector: 'app-horizontal-module-nav',
  imports: [MatIconModule, MatButtonModule, MatMenuModule, TranslatePipe, NotificationBadge, MoreMenu, ModuleGridMenu],
  templateUrl: './horizontal-module-nav.html',
  styleUrl: './horizontal-module-nav.scss'
})
export class HorizontalModuleNav extends ModuleNavBase {
  private destroyRef = inject(DestroyRef);
  private navEl = viewChild.required<ElementRef<HTMLElement>>('navEl');

  private visibleCount = signal(Number.MAX_SAFE_INTEGER);

  protected visibleModules = computed(() => this.modules().slice(0, this.visibleCount()));

  private resizeObserver = new ResizeObserver(entries => {
    const navWidth = entries[0].contentRect.width;
    const count = Math.max(0, Math.ceil(navWidth / MIN_ITEM_WIDTH) - 2);
    this.visibleCount.set(count);
  });

  constructor() {
    super();
    afterNextRender(() => {
      this.resizeObserver.observe(this.navEl().nativeElement);
      this.destroyRef.onDestroy(() => this.resizeObserver.disconnect());
    });
  }
}
