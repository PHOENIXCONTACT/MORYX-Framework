
import { Component, inject, Input, input, OnInit, output, signal } from '@angular/core';
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
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { MaterialFlowService } from 'src/app/services/material-flow.service';
import { ReferenceType } from 'src/app/models/material-container';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
@Component({
  selector: 'app-card',
  imports: [MatAnchor, MatIconModule, MatButtonModule, MatCardModule, MatChipsModule, TranslateModule, CommonModule],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent implements OnInit{
  container = input.required<MaterialContainerModel>();
  delete = output<number>();
  private dialog = inject(MatDialog);
  private materialApi = inject(MaterialManagementService)
  private snackbarService = inject(SnackbarService);
  private translateService = inject(TranslateService);
  protected translationConstants = TranslationConstants;
  private translations  = signal<{ [key: string]: string }>({});

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
          .then(async () => {
            const translations = await this.getTranslations();
            this.snackbarService.showSuccess(translations[TranslationConstants.CARD.ADVISED]);
          })
      }
    })
  }

  ngOnInit(): void {
    this.getTranslations().then(result => this.translations.set(result)); 
  }

  link() {
    const dialogRef = this.dialog.open(DialogContainerLinkingComponent);
    dialogRef.afterClosed().subscribe(data => {
      if (data) {
      }
    })
  }

  async getTranslations(): Promise<{ [key: string]: string }> {
    return await lastValueFrom(this.translateService
      .get([
        TranslationConstants.CARDS.DELETED,
        TranslationConstants.SUMMARIES.ORDER
      ]));
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
    if ((reference as any).type?.toLowerCase().includes(ReferenceType.Order.toLowerCase())) {
      const result=   this.translations()[TranslationConstants.SUMMARIES.ORDER] + ' : ' + ((reference as OrderReferenceModel).orderNumber ?? 'NA');
      console.log("reesult",result);
      return result;
    }
    return "?";
  }
}