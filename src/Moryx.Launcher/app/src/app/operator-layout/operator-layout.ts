/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, signal } from '@angular/core';
import { HorizontalModuleNav } from '../horizontal-module-nav/horizontal-module-nav';

@Component({
  selector: 'app-operator-layout',
  imports: [HorizontalModuleNav],
  templateUrl: './operator-layout.html',
  styleUrl: './operator-layout.scss',
})
export class OperatorLayout {
  hasRightRegion = signal(false);

  onRightRegionSlotChange(event: Event): void {
    const slot = event.target as HTMLSlotElement;
    this.hasRightRegion.set(slot.assignedNodes().length > 0);
  }
}
