/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, OnInit, signal, ChangeDetectionStrategy } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from "@angular/material/dialog";
import { TranslatePipe } from "@ngx-translate/core";
import { Observable } from "rxjs";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { OperationViewModel } from "@app/models/operation-view-model";
import { ConfirmationType, ReportModel, ReportContext } from '@api/models';
import { CommonModule } from "@angular/common";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatFormFieldModule } from "@angular/material/form-field";
import { FormsModule } from "@angular/forms";
import { MatRadioButton, MatRadioGroup } from "@angular/material/radio";
import { MatListModule } from "@angular/material/list";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatButtonModule } from "@angular/material/button";
import { MatInputModule } from "@angular/material/input";

@Component({
  selector: "app-report-dialog",
  templateUrl: "./report-dialog.html",
  styleUrls: ["./report-dialog.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    CommonModule,
    MatDialogModule,
    TranslatePipe,
    MatGridListModule,
    MatFormFieldModule,
    FormsModule,
    MatRadioButton,
    MatListModule,
    MatProgressBarModule,
    MatButtonModule,
    MatInputModule,
    MatRadioGroup
  ]
})
export class ReportDialog implements OnInit {
  protected reportContext = signal<ReportContext | undefined>(undefined);
  protected isLoading = signal(false);
  protected success = signal(0);
  protected scrap = signal(0);
  protected comment = signal("");
  protected confirmationType = signal<"partial" | "final">("partial");
  protected estimatedSuccess = computed(() => this.success() + (this.reportContext()?.reportedSuccess ?? 0));
  protected estimatedFailure = computed(() => this.scrap() + (this.reportContext()?.reportedFailure ?? 0));
  protected canReport = computed(() => {
    if (this.success() < 0 || this.scrap() < 0) {
      return false;
    }

    if (this.confirmationType() == "partial" && !this.reportContext()?.canPartial) {
      return false;
    }

    if (this.confirmationType() == "final" && !this.reportContext()?.canFinal) {
      return false;
    }

    return true;
  })

  protected TranslationConstants = TranslationConstants;

  private dialog = inject(MatDialogRef<ReportDialog>);
  protected data = inject<ReportDialogData>(MAT_DIALOG_DATA);

  async ngOnInit() {
    this.isLoading.update(_=> true);
    const result = await this.data
      .onGetContext(this.data.operation.model.identifier!)
      .toAsync();
    this.reportContext.update(_=> result);
    this.success.update(_=> this.reportContext()?.unreportedSuccess ?? 0);
    this.scrap.update(_=> this.reportContext()?.unreportedFailure ?? 0);
    this.isLoading.update(_=> false);
    if (this.reportContext()?.canPartial) {
      this.confirmationType.update(_=> "partial");
    }
    else {
      this.confirmationType.update(_=> "final");
    }
  }

  protected async submit(): Promise<void> {
    this.isLoading.update(_=> true);

    const report = <ReportModel>{
      successCount: this.success(),
      failureCount: this.scrap(),
      comment: this.comment(),
      confirmationType:
        this.confirmationType() === "partial"
          ? ConfirmationType.Partial
          : ConfirmationType.Final,
    };
    let failed = false;

    await this.data
      .onSubmit(this.data.operation.model.identifier!, report)
      .toAsync()
      .catch(() => {
        failed = true;
        this.isLoading.update(_=> false);
      });
    if (!failed) {
      this.dialog.close();
    }
  }
}

export interface ReportDialogData {
  operation: OperationViewModel;
  onSubmit: (guid: string, body: ReportModel) => Observable<void>;
  onGetContext: (guid: string) => Observable<ReportContext>;
}

