import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { PreAdviceDepartureReasonModel, PreAdvideModel, ResourceModel } from 'src/app/api/models';
import { MaterialManagementService } from 'src/app/api/services';
import { MatFormField, MatInputModule, MatLabel } from "@angular/material/input";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-dialog-pre-advice',
  imports: [MatDialogModule, MatButtonModule, MatLabel, MatSelectModule],
  templateUrl: './dialog-pre-advice.component.html',
  styleUrl: './dialog-pre-advice.component.scss',
})
export class DialogPreAdviceComponent {
  private data = inject<ResourceModel>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<DialogPreAdviceComponent>);
  private materialApi = inject(MaterialManagementService)
  private snackbarService = inject(SnackbarService);

  reason = signal<PreAdviceDepartureReasonModel | undefined>(undefined);

  reasons() {
    return Object.keys(PreAdviceDepartureReasonModel);
  }

  advice() {
    const request: PreAdvideModel = {
      containerId: this.data.id,
      departureReason: this.reason()
    };
    const response = firstValueFrom(this.materialApi.preAdviceAsync({ body: request }));
    response.catch((e: HttpErrorResponse) => {
      this.snackbarService.processStatusCodes(e);
    })
    .then(() => {
      this.snackbarService.showSuccess("Advice done!");
      this.dialogRef.close();
    })
  }
}
