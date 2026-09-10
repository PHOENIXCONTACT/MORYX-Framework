/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, input, ChangeDetectionStrategy } from '@angular/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '../translation-constants';

@Component({
  selector: 'app-multi-progress-bar',
  standalone: true,
  templateUrl: './multi-progress-bar.html',
  styleUrls: ['./multi-progress-bar.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatTooltipModule,
    TranslatePipe
  ]
})
export class MultiProgressBar {
  readonly totalAmount = input.required<number>();
  readonly successCount = input<number>(0);
  readonly scrapCount = input<number>(0);
  readonly activeCount = input<number>(0);
  readonly pendingCount = input<number>(0);

  // Customization
  readonly activeLabel = input.required<string>();

  protected TranslationConstants = TranslationConstants;

  protected successPercent = computed(() => this.calculatePercent(this.successCount()));
  protected scrapPercent = computed(() => this.calculatePercent(this.scrapCount()));
  protected activePercent = computed(() => this.calculatePercent(this.activeCount()));
  protected pendingPercent = computed(() => this.calculatePercent(this.pendingCount()));

  protected residualCount = computed(() => {
    const residual = this.totalAmount() - this.successCount() - this.scrapCount() - this.activeCount() - this.pendingCount();
    return residual < 0 ? 0 : residual;
  });

  protected residualPercent = computed(() => {
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
