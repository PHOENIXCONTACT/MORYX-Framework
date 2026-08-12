/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from "@angular/common";
import { Component, inject, signal, ChangeDetectionStrategy } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/translation-constants";
import { InterruptDialogData } from "./interrupt-dialog-data";
import { DialogContext } from "@app/components/dialog-context/dialog-context";

@Component({
  selector: "app-interrupt-dialog",
  templateUrl: "./interrupt-dialog.html",
  styleUrls: ["./interrupt-dialog.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    CommonModule,
    MatDialogModule,
    TranslatePipe,
    MatProgressBarModule,
    MatButtonModule,
    DialogContext
  ]
})
export class InterruptDialog {

  protected isLoading = signal(false);
  protected TranslationConstants = TranslationConstants;

  private dialog = inject(MatDialogRef<InterruptDialog>);
  protected data = inject<InterruptDialogData>(MAT_DIALOG_DATA);

  constructor() {
  }

  protected async submit(): Promise<void> {
    this.isLoading.set(true);
    let failed = false;

    await this.data
      .onSubmit(this.data.operation.model.identifier!, undefined)
      .catch(() => {
        failed = true;
        this.isLoading.set(false);
      });
    if (!failed) {
      this.dialog.close();
    }
  }
}

