/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { afterNextRender, Component, computed, DestroyRef, ElementRef, inject, signal, viewChild } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { WebModuleItem } from '../models/web-module-item';
import { ExternalModuleItem } from '../models/external-module-item';
import { ModuleItem } from '../models/module-item';
import { NotificationBadge } from '../notification-badge/notification-badge';
import { MoreMenu } from '../more-menu/more-menu';
import { ModuleGridMenu } from '../module-grid-menu/module-grid-menu';
import { TranslatePipe } from '@ngx-translate/core';
import { ModuleService } from '../services/module.service';
import { LocationPersistenceService } from '../services/location-persistence.service';
import { TranslationConstants } from '../translation-constants';

const MIN_ITEM_WIDTH = 112; // items shrink below this -> remove from the end

@Component({
  selector: 'app-horizontal-module-nav',
  imports: [MatIconModule, MatButtonModule, MatMenuModule, TranslatePipe, NotificationBadge, MoreMenu, ModuleGridMenu],
  templateUrl: './horizontal-module-nav.html',
  styleUrl: './horizontal-module-nav.scss'
})
export class HorizontalModuleNav {
  private moduleService = inject(ModuleService);
  private locationPersistenceService = inject(LocationPersistenceService);
  private destroyRef = inject(DestroyRef);

  modules = this.moduleService.userModules;
  activeRoute = this.moduleService.activeRoute;

  protected TranslationConstants = TranslationConstants;
  private navEl = viewChild.required<ElementRef<HTMLElement>>('navEl');

  private visibleCount = signal(Number.MAX_SAFE_INTEGER);

  visibleModules = computed(() => this.modules().slice(0, this.visibleCount()));

  private resizeObserver = new ResizeObserver(entries => {
    const navWidth = entries[0].contentRect.width;
    const count = Math.max(0, Math.floor(navWidth / MIN_ITEM_WIDTH) - 1);
    this.visibleCount.set(count);
  });

  constructor() {
    afterNextRender(() => {
      this.resizeObserver.observe(this.navEl().nativeElement);
      this.destroyRef.onDestroy(() => this.resizeObserver.disconnect());
    });
  }

  resolveHref(module: ModuleItem): string {
    return this.locationPersistenceService.resolveHref(module);
  }

  asWeb(item: WebModuleItem | ExternalModuleItem): WebModuleItem | null {
    return 'eventStream' in item ? item as WebModuleItem : null;
  }
}
