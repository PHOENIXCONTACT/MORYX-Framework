/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ChangeDetectionStrategy, input, signal } from "@angular/core";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { OperationModel } from "@app/api/models";
import { TranslatePipe } from "@ngx-translate/core";
import { CommonModule } from "@angular/common";
import { MatTooltip } from "@angular/material/tooltip";

@Component({
  selector: "app-dialog-context",
  templateUrl: "./dialog-context.html",
  styleUrls: ["./dialog-context.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    TranslatePipe,
    CommonModule,
    MatTooltip
  ]
})
export class DialogContext {
  protected TranslationConstants = TranslationConstants;
  operationModel = input.required<OperationModel>();
  orderExpanded = signal(false);
  productExpanded = signal(false);

  toggleOrderExpansion(): void {
    this.orderExpanded.update(v => !v);
  }

  toggleProductExpansion(): void {
    this.productExpanded.update(v => !v);
  }
}
