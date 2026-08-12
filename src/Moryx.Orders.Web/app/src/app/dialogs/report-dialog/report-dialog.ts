/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, OnInit, signal, ChangeDetectionStrategy } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from "@angular/material/dialog";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/translation-constants";
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
import { MatIconModule } from "@angular/material/icon";
import { OperatorSelector } from "@app/components/operator-selector/operator-selector";
import { DialogContext } from "@app/components/dialog-context/dialog-context";

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
    MatRadioGroup,
    MatIconModule,
    OperatorSelector,
    DialogContext
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
  protected selectedOperatorId = signal<string|null>(null);
  protected creatingOperatorFailed = signal<boolean>(false);

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
    this.isLoading.set(true);
    const result = await this.data
      .onGetContext(this.data.operation.model.identifier!);
    this.reportContext.set(result);
    this.success.set(this.reportContext()?.unreportedSuccess ?? 0);
    this.scrap.set(this.reportContext()?.unreportedFailure ?? 0);
    this.isLoading.set(false);
    if (this.reportContext()?.canPartial) {
      this.confirmationType.set("partial");
    }
    else {
      this.confirmationType.set("final");
    }
  }


  protected async submit(): Promise<void> {
    if (this.creatingOperatorFailed()) {
      return;
    }

    this.isLoading.set(true);

    const report = <ReportModel>{
      successCount: this.success(),
      failureCount: this.scrap(),
      comment: this.comment(),
      confirmationType:
        this.confirmationType() === "partial"
          ? ConfirmationType.Partial
          : ConfirmationType.Final,
      userIdentifier: this.selectedOperatorId()
    };

    let failed = false;

    await this.data
      .onSubmit(this.data.operation.model.identifier!, report)
      .catch(() => {
        failed = true;
        this.isLoading.set(false);
      });
    if (!failed) {
      this.dialog.close();
    }
  }
}

export interface ReportDialogData {
  operation: OperationViewModel;
  onSubmit: (guid: string, body: ReportModel) => Promise<void>;
  onGetContext: (guid: string) => Promise<ReportContext>;
}

