/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, linkedSignal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { PartConnector, PartModel, ProductModel } from '../../../api/models';
import { DialogAddPart } from '../../../dialogs/dialog-add-part/dialog-add-part';
import { EditProductsService } from '../../../services/edit-products.service';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { ProductPartsDetailsComponent } from './product-parts-details/product-parts-details';
import { DefaultView } from '../../default-view/default-view';
import { MatButtonModule } from '@angular/material/button';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-product-parts',
  templateUrl: './product-parts.html',
  styleUrls: ['./product-parts.scss'],
  imports: [
    CommonModule,
    MatExpansionModule,
    MatListModule,
    ProductPartsDetailsComponent,
    DefaultView,
    MatButtonModule,
    TranslateModule
  ]
})
export class ProductParts {
  private editProductsService = inject(EditProductsService);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });
  currentProduct = toSignal(this.editProductsService.currentProduct$);
  expandedPart = linkedSignal(this.editProductsService.currentPartConnector);
  selectedPart = linkedSignal(this.editProductsService.currentPart);
  TranslationConstants = TranslationConstants;

  onSelectPartConnector(connector: PartConnector) {
    const firstPartId = connector.parts && connector.parts.length > 0 ? connector.parts[0].id : 0;
    this.router.navigate(['details', this.currentProduct()!.id, 'parts', connector.name, firstPartId]);
  }

  onDeselectPartConnector(part: PartConnector) {
    if (part.name !== this.expandedPart()?.name) return;
    this.router.navigate(['details', this.currentProduct()!.id, 'parts', 'base', 0]);
  }

  onSelectPartElement(part: PartModel) {
    this.router.navigate(['details', this.currentProduct()!.id, 'parts', this.expandedPart()!.name, part.id]);
  }

  async addPart() {
    const connector = this.expandedPart();
    const dialogRef = this.dialog.open(DialogAddPart, { data: connector });

    const product = await firstValueFrom(dialogRef.afterClosed());
    if (!product) return;

    // Create new Part
    let newPart = <PartModel>{};
    newPart.product = product;
    if (connector?.propertyTemplates) {
      newPart.properties = structuredClone(connector.propertyTemplates!);
    }

    const addedPart = this.editProductsService.addPartToConnector(newPart);
    this.router.navigate(['details', this.currentProduct()!.id, 'parts', connector!.name, addedPart.id]);
  }

  removePart() {
    const connector = this.expandedPart();
    if (!connector) {
      return;
    } 

    this.editProductsService.removePartFromConnector();

    if (connector?.isCollection) {
      this.onSelectPartConnector(connector);
    } else {
      this.onDeselectPartConnector(connector);
    }
  }

  createProductNameWithIdentity(product: ProductModel | undefined, shortened: boolean = false, maxLength: number = 40): string {
    return this.editProductsService.createProductNameWithIdentity(product, shortened, maxLength);
  }
}
