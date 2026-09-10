/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ChangeDetectionStrategy, input } from "@angular/core";
import { TranslationConstants } from "@app/translation-constants";
import { OperationModel } from "@app/api/models";
import { TranslatePipe } from "@ngx-translate/core";
import { DecimalPipe } from "@angular/common";

@Component({
  selector: "app-dialog-context",
  templateUrl: "./dialog-context.html",
  styleUrls: ["./dialog-context.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    TranslatePipe,
    DecimalPipe
  ]
})
export class DialogContext {
  protected TranslationConstants = TranslationConstants;
  operationModel = input.required<OperationModel>();
}
