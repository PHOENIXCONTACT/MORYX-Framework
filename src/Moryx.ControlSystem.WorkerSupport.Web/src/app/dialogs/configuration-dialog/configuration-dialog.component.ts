import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { WorkerSupportService } from 'src/app/api/services/worker-support.service';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';

@Component({
    selector: 'app-configuration-dialog',
    templateUrl: './configuration-dialog.component.html',
    styleUrls: ['./configuration-dialog.component.scss'],
    imports: [
      CommonModule,
      MatSelectionList,
      MatListOption,
      MatDialogModule,
      TranslateModule,
      MatProgressSpinnerModule
    ],
    standalone: true
})
export class ConfigurationDialogComponent implements OnInit {
  instructors: string[] | undefined = undefined;
  TranslationConstants = TranslationConstants;

  constructor(
    public dialogRef: MatDialogRef<ConfigurationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private workerSupportService: WorkerSupportService,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService
  ) {}

  ngOnInit(): void {
    this.workerSupportService.getInstructors().subscribe({
      next: (result) => this.instructors = result.sort((a,b) => a.localeCompare(b)),
      error: async (e: HttpErrorResponse) =>
        await this.moryxSnackbar.handleError(e),
    });
  }

  saveName(name: string) {
    this.data.instructorName = name;
  }
}

export interface DialogData {
  instructorName: string;
}
