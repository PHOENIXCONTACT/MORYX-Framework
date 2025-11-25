import { Component, computed, Inject, OnInit, signal } from "@angular/core";
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from "@angular/material/dialog";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { Observable } from "rxjs";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { OperationViewModel } from "../../models/operation-view-model";
import { ConfirmationType } from '../../api/models';
import { ReportModel } from '../../api/models';
import { ReportContext } from '../../api/models';
import { CommonModule } from "@angular/common";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatFormFieldModule } from "@angular/material/form-field";
import { FormsModule } from "@angular/forms";
import { MatRadioButton } from "@angular/material/radio";
import { MatListModule } from "@angular/material/list";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatButtonModule } from "@angular/material/button";
import { MatInputModule } from "@angular/material/input";

@Component({
  selector: "app-report-dialog",
  templateUrl: "./report-dialog.component.html",
  styleUrls: ["./report-dialog.component.scss"],
  standalone: true,
  imports: [
    CommonModule, 
    MatDialogModule,
    TranslateModule,
    MatGridListModule,
    MatFormFieldModule,
    FormsModule,
    MatRadioButton,
    MatListModule,
    MatProgressBarModule,
    MatButtonModule,
    MatInputModule
  ],
})
export class ReportDialogComponent implements OnInit {
  context = signal<ReportContext | undefined>(undefined);
  isLoading = signal(false);
  success = signal(0);
  scrap = signal(0);
  comment = signal("");
  confirmationType = signal("");
  estimatedSuccess = computed(() => this.success() + (this.context()?.reportedSuccess ?? 0));
  estimatedFailure = computed(() => this.scrap() + (this.context()?.reportedFailure ?? 0));
  canReport = computed(() => {
    if (this.success() < 0 || this.scrap() < 0) return false;

    if (this.confirmationType() == "partial" && !this.context()?.canPartial)
      return false;

    if (this.confirmationType() == "final" && !this.context()?.canFinal)
      return false;

    return true;
  })
  TranslationConstants = TranslationConstants;

  constructor(
    private dialog: MatDialogRef<ReportDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ReportDialogData,
    public translate: TranslateService
  ) {}

  async ngOnInit() {
    this.isLoading.update(_=> true);
    const result = await this.data
      .onGetContext(this.data.operation.model.identifier!)
      .toAsync();
    this.context.update(_=> result);
    this.success.update(_=> this.context()?.unreportedSuccess ?? 0);
    this.scrap.update(_=> this.context()?.unreportedFailure ?? 0);
    this.isLoading.update(_=> false);
    if (this.context()?.canPartial) this.confirmationType.update(_=> "partial");
    else this.confirmationType.update(_=> "final");
  }

  async submit(): Promise<void> {
    this.isLoading.update(_=> true);

    let report = <ReportModel>{
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
    if (!failed) this.dialog.close();
  }
}

export interface ReportDialogData {
  operation: OperationViewModel;
  onSubmit: (guid: string, body: ReportModel) => Observable<void>;
  onGetContext: (guid: string) => Observable<ReportContext>;
}
