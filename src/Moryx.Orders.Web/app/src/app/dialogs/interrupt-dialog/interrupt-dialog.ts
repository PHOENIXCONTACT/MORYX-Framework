/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Inject, signal } from "@angular/core";
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from "@angular/material/dialog";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";

import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatButtonModule } from "@angular/material/button";
import { InterruptDialogData } from "./interrupt-dialog-data";

@Component({
  selector: "app-interrupt-dialog",
  templateUrl: "./interrupt-dialog.html",
  styleUrls: ["./interrupt-dialog.scss"],
  standalone: true,
  imports: [
    MatDialogModule,
    TranslateModule,
    MatProgressBarModule,
    MatButtonModule
],
})
export class InterruptDialog {
  
  isLoading = signal(false);
  TranslationConstants = TranslationConstants;

  constructor(
    private dialog: MatDialogRef<InterruptDialog>,
    @Inject(MAT_DIALOG_DATA) public data: InterruptDialogData,
    public translate: TranslateService
  ) {}

  async submit(): Promise<void> {
    this.isLoading.update(_=> true);
    let failed = false;

    await this.data
      .onSubmit(this.data.operation.model.identifier!, undefined)
      .toAsync()
      .catch(() => {
        failed = true;
        this.isLoading.update(_=> false);
      });
    if (!failed) this.dialog.close();
  }
}

