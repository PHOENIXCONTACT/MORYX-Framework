
import { Component, inject, Input, input, OnInit, output } from '@angular/core';
import { MatAnchor, MatButtonModule } from "@angular/material/button";
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { DialogPreAdviceComponent } from 'src/app/dialogs/dialog-pre-advice/dialog-pre-advice.component';
import { MaterialContainerModel, OrderReferenceModel, PreAdviceModel, ReferenceModel, ResourceModel, ResourceTypeModel } from 'src/app/api/models';
import { DialogContainerLinkingComponent } from 'src/app/dialogs/dialog-container-linking/dialog-container-linking.component';
import { DialogConfirmDeleteComponent } from 'src/app/dialogs/dialog-confirm-delete/dialog-confirm-delete.component';
import { MaterialManagementService } from 'src/app/api/services';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { MaterialFlowService } from 'src/app/services/material-flow.service';
@Component({
  selector: 'app-card',
  imports: [MatAnchor, MatIconModule, MatButtonModule, MatCardModule, MatChipsModule],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent {
  container = input.required<MaterialContainerModel>();
  private dialog = inject(MatDialog);
  private materialApi = inject(MaterialManagementService)
  private materialFlow = inject(MaterialFlowService);
  private snackbarService = inject(SnackbarService);

  preAdvice() {
    const dialogRef = this.dialog.open(DialogPreAdviceComponent, {
      data: this.container()
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        const request = result as PreAdviceModel;
        const response = firstValueFrom(this.materialApi.preAdviceAsync({ body: request }));
        response.catch((e: HttpErrorResponse) => {
          this.snackbarService.processStatusCodes(e);
        })
          .then(() => {
            this.snackbarService.showSuccess("Advice done!");
          })
      }
    })
  }

  link() {
    const dialogRef = this.dialog.open(DialogContainerLinkingComponent);
    dialogRef.afterClosed().subscribe(data => {
      if (data) {
      }
    })
  }

  onDelete() {
    var dialogRef = this.dialog.open(DialogConfirmDeleteComponent);
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.delete.emit(this.container().id ?? 0);
      }
    })
  }

  materialReferenceToString(reference: ReferenceModel): string {
    if (reference.fullName?.toLowerCase().includes("orders")) {
      return 'Order : ' + ((reference as OrderReferenceModel).orderNumber ?? 'NA');
    }
    return "?";
  }
}
