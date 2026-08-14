/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, linkedSignal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { PartConnector, PartModel } from '@api/models';
import { DialogAddPart } from '@app/dialogs/dialog-add-part/dialog-add-part';
import { EditProductsService } from '@app/services/edit-products.service';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { ProductPartsDetailsComponent } from './product-parts-details/product-parts-details';
import { MatButtonModule } from '@angular/material/button';
import { firstValueFrom } from 'rxjs';
import { MatIcon } from '@angular/material/icon';
import { MatTooltip } from "@angular/material/tooltip";

@Component({
  selector: 'app-product-parts',
  templateUrl: './product-parts.html',
  styleUrls: ['./product-parts.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    CommonModule,
    MatExpansionModule,
    MatListModule,
    ProductPartsDetailsComponent,
    MatButtonModule,
    TranslatePipe,
    MatIcon,
    MatTooltip,
    RouterLink,
]
})
export class ProductParts {
  private editProductsService = inject(EditProductsService);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  protected isEditMode = this.editProductsService.editing;
  protected currentProduct = this.editProductsService.currentProduct;
  protected expandedPart = linkedSignal(this.editProductsService.currentPartConnector);
  protected selectedPart = linkedSignal(this.editProductsService.currentPart);
  protected TranslationConstants = TranslationConstants;

  protected getPartRoute(part: PartModel): string[] {
    return ['/', 'details', this.currentProduct()!.id!.toString(), 'parts', this.expandedPart()!.name!, part.id!.toString()];
  }

  protected onSelectPartConnector(connector: PartConnector) {
    // Skip navigation if current part already belongs to this connector (e.g. on initial page load)
    const currentPart = this.selectedPart();
    const alreadyOnConnector = currentPart && connector.parts?.some(p => p.id === currentPart.id);
    if (alreadyOnConnector) {
      return;
    }

    const firstPartId = connector.parts && connector.parts.length > 0 ? connector.parts[0].id : 0;
    this.router.navigate(['details', this.currentProduct()!.id, 'parts', connector.name, firstPartId]);
  }

  protected onDeselectPartConnector(part: PartConnector) {
    if (part.name !== this.expandedPart()?.name) {
      return;
    }
    this.router.navigate(['details', this.currentProduct()!.id, 'parts', 'base', 0]);
  }

  protected async addPart() {
    const connector = this.expandedPart();
    const dialogRef = this.dialog.open(DialogAddPart, { data: connector });

    const product = await firstValueFrom(dialogRef.afterClosed());
    if (!product) {
      return;
    }

    // Create new Part
    const newPart = <PartModel>{};
    newPart.product = product;
    if (connector?.propertyTemplates) {
      newPart.properties = structuredClone(connector.propertyTemplates!);
    }

    const addedPart = this.editProductsService.addPartToConnector(newPart);
    this.router.navigate(['details', this.currentProduct()!.id, 'parts', connector!.name, addedPart.id]);
  }

  protected removePart() {
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

  protected getConnectorPreview(connector: PartConnector): string {
    if (!connector.parts || connector.parts.length === 0) {
      return '';
    }
    const partNames = connector.parts.map(p => p.product
      ? this.editProductsService.createProductNameWithIdentity(p.product) :
      'Unnamed Product');
    return partNames.join(', ');
  }

  protected openProduct(part: PartModel) {
    if (this.isEditMode()) {
      this.router.navigate(['details', this.currentProduct()!.id, 'parts', this.expandedPart()!.name, part.id], { queryParamsHandling: 'preserve' });
      return;
    }
    this.router.navigate(['details', part.product?.id]);
  }
}
