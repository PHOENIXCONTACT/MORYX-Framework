import { Component, Inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { DuplicateProductInfos } from 'src/app/models/DuplicateProductInfos';
import { ProductModel } from '../../api/models';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-dialog-duplicate-product',
    templateUrl: './dialog-duplicate-product.component.html',
    styleUrls: ['./dialog-duplicate-product.component.scss'],
    standalone: true,
    imports: [
      CommonModule,
      MatFormFieldModule,
      FormsModule,
      TranslateModule,
      MatDialogModule,
      MatInputModule,
      MatButtonModule
    ]
})
export class DialogDuplicateProductComponent {
  productToDuplicate = signal<ProductModel | undefined>(undefined);
  duplicateInfos = signal<DuplicateProductInfos | undefined>(undefined);
  TranslationConstants = TranslationConstants;

  constructor(
    public dialogRef: MatDialogRef<DialogDuplicateProductComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductModel,
    public translate: TranslateService
  ) {
    this.productToDuplicate.update(_=> data);
    this.duplicateInfos.update(_=> {
      return { product: data } as DuplicateProductInfos
    });
  }

  onClose() {
    this.dialogRef.close();
  }
}
