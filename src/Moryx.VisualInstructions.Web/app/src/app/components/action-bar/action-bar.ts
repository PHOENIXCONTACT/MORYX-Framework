/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, input, output } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { InstructionResultModel } from "@api/models";

@Component({
  selector: "app-action-bar",
  templateUrl: "./action-bar.html",
  styleUrls: ["./action-bar.scss"],
  imports: [
    MatButtonModule,
    MatIconModule
  ]
})
export class ActionBar {
  readonly instructionCount = input.required<number>();
  readonly activeIndex = input.required<number>();
  readonly results = input<InstructionResultModel[]>([]);

  readonly previous = output<void>();
  readonly next = output<void>();
  readonly viewNewest = output<void>();
  readonly selectResult = output<InstructionResultModel>();

  protected showNavigation = computed(() => {
    return this.instructionCount() > 1
  });

  protected hasPrevious = computed(() => {
    return this.activeIndex() > 0
  });

  protected hasNext = computed(() =>  {
    return this.activeIndex() < this.instructionCount() - 1
  });
}
