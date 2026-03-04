/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, input } from '@angular/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from '../extensions/translation-constants.extensions';

@Component({
  selector: 'app-multi-progress-bar',
  standalone: true,
  templateUrl: './multi-progress-bar.html',
  styleUrls: ['./multi-progress-bar.scss'],
  imports: [
    MatTooltipModule,
    TranslateModule
  ]
})
export class MultiProgressBar {
  totalAmount = input.required<number>();
  successCount = input<number>(0);
  scrapCount = input<number>(0);
  activeCount = input<number>(0);
  pendingCount = input<number>(0);

  // Customization
  activeLabel = input.required<string>();

  TranslationConstants = TranslationConstants;

  successPercent = computed(() => this.calculatePercent(this.successCount()));
  scrapPercent = computed(() => this.calculatePercent(this.scrapCount()));
  activePercent = computed(() => this.calculatePercent(this.activeCount()));
  pendingPercent = computed(() => this.calculatePercent(this.pendingCount()));

  residualCount = computed(() => {
    const residual = this.totalAmount() - this.successCount() - this.scrapCount() - this.activeCount() - this.pendingCount();
    return residual < 0 ? 0 : residual;
  });

  residualPercent = computed(() => {
    // Hide residual segment when count is 0 (CSS flex-grow handles the gap)
    if (this.residualCount() === 0) {
      return 0;
    }
    const residual = 100 - this.successPercent() - this.scrapPercent() - this.activePercent() - this.pendingPercent();
    return residual < 0 ? 0 : residual;
  });

  private calculatePercent(count: number): number {
    const total = this.totalAmount();
    if (!total || count <= 0) {
      return 0;
    }
    return Math.round((count * 100) / total);
  }
}
