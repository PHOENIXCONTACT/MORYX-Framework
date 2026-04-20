/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { TranslateModule } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { InterruptDialogData } from "./interrupt-dialog-data";

@Component({
  selector: "app-interrupt-dialog",
  templateUrl: "./interrupt-dialog.html",
  styleUrls: ["./interrupt-dialog.scss"],
  imports: [
    CommonModule,
    MatDialogModule,
    TranslateModule,
    MatProgressBarModule,
    MatButtonModule
  ]
})
export class InterruptDialog {

  isLoading = signal(false);
  TranslationConstants = TranslationConstants;

  private dialog = inject(MatDialogRef<InterruptDialog>);
  data = inject<InterruptDialogData>(MAT_DIALOG_DATA);

  constructor() {
  }

  async submit(): Promise<void> {
    this.isLoading.set(true);
    let failed = false;

    await this.data
      .onSubmit(this.data.operation.model.identifier!, undefined)
      .toAsync()
      .catch(() => {
        failed = true;
        this.isLoading.set(false);
      });
    if (!failed) this.dialog.close();
  }
}

