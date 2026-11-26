import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { MatFormField, MatInputModule, MatLabel } from "@angular/material/input";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { TranslatePipe } from '@ngx-translate/core';
import { MaterialContainerModel, PreAdviceDepartureReasonModel, PreAdviceModel } from '@app/api/models';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';

@Component({
  selector: 'app-dialog-pre-advice',
  imports: [MatDialogModule, MatButtonModule, MatLabel, MatSelectModule, TranslatePipe, MatInputModule],
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
