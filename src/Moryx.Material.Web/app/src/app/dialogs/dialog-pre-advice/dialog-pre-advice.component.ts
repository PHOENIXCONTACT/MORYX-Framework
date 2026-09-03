import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { MaterialContainerModel, PreAdviceDepartureReasonModel, PreAdviceModel, ResourceModel } from 'src/app/api/models';
import { MaterialManagementService } from 'src/app/api/services';
import { MatFormField, MatInputModule, MatLabel } from "@angular/material/input";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-dialog-pre-advice',
  imports: [MatDialogModule, MatButtonModule, MatLabel, MatSelectModule, TranslateModule, MatInputModule],
  templateUrl: './dialog-pre-advice.component.html',
  styleUrl: './dialog-pre-advice.component.scss',
})
export class DialogPreAdviceComponent {
  private dialogRef = inject(MatDialogRef<DialogPreAdviceComponent>);
  protected data = inject<MaterialContainerModel>(MAT_DIALOG_DATA);
  protected translationConstants = TranslationConstants;
  reason = signal<PreAdviceDepartureReasonModel | undefined>(undefined);

  reasons() {
    return Object.keys(PreAdviceDepartureReasonModel);
  }

  advice() {
    const request: PreAdviceModel = {
      containerId: this.data.id,
      departureReason: this.reason()
    };
    this.dialogRef.close(request);
  }
}
