/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { PartConnector, PartModel, ProductModel } from '../../../api/models';
import { DialogAddPartComponent } from '../../../dialogs/dialog-add-part/dialog-add-part';
import { CacheProductsService } from '../../../services/cache-products.service';
import { EditProductsService } from '../../../services/edit-products.service';
import { CommonModule } from '@angular/common';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { ProductPartsDetailsComponent } from './product-parts-details/product-parts-details';
import { DefaultView } from '../../default-view/default-view';
import { MatButtonModule } from '@angular/material/button';

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
  ],
  standalone: true
})
export class ProductParts implements OnInit {
  currentProduct = signal<ProductModel | undefined>(undefined);
  expandedPart = signal<PartConnector | undefined>(undefined);
  selectedPart = signal<PartModel | undefined>(undefined);
  TranslationConstants = TranslationConstants;

  constructor(
    public editService: EditProductsService,
    private cacheService: CacheProductsService,
    private router: Router,
    private route: ActivatedRoute,
    public dialog: MatDialog,
    public translate: TranslateService
  ) {
    this.editService.currentProduct.subscribe((product) => {
      this.currentProduct.set(product);
      this.init();
    });
  }

  ngOnInit(): void {
    this.init();
  }

  init() {
    const partName = this.route.snapshot.paramMap.get('partName');

    if (partName === 'base') return;

    if (!this.currentProduct) {
      this.router.navigate(['']);
      return;
    }

    let url = this.getBaseUrl();
    this.expandedPart.set(this.currentProduct()?.parts?.find(
      (p) => p.name === partName
    ));
    if (!this.expandedPart()) {
      url += 'base/0';
      this.router.navigate([url]);
      return;
    }
    const partId = Number(this.route.snapshot.paramMap.get('partId'));
    if (partId === 0) {
      return;
    }

    this.selectedPart.set(this.expandedPart()?.parts?.find(
      (part) => part.id === partId
    ));
    if (!this.selectedPart() && this.expandedPart()?.name) {
      if (this.expandedPart()?.isCollection) {
        url += this.expandedPart()?.name + '/0';
        this.router.navigate([url]);
      } else {
        this.expandedPart.set(undefined);
        url += 'base/0';
        this.router.navigate([url]);
        return;
      }
    }
  }

  onSelectPartConnector(part: PartConnector) {
    if (this.expandedPart()?.name === part.name) return;
    this.expandedPart.set(part);
    let url = this.getBaseUrl();
    url += part.name;
    if (!part.isCollection && part.parts?.length) {
      this.selectedPart.set(part.parts?.[0]);
      url += '/' + this.selectedPart()?.id;
    } else {
      url += '/0';
      this.selectedPart.set(undefined);
    }
    this.router.navigate([url]);
  }

  onDeselectPartConnector(part: PartConnector) {
    if (part.name !== this.expandedPart()?.name) return;
    this.expandedPart.set(undefined);
    this.selectedPart.set(undefined);
    const url = this.getBaseUrl() + 'base/0';
    this.router.navigate([url]);
  }

  onSelectPartElement(part: PartModel) {
    this.selectedPart.set(part);
    let url = this.getBaseUrl();
    url += this.expandedPart()?.name + '/' + part.id;
    this.router.navigate([url]);
  }

  addPart() {
    const dialogRef = this.dialog.open(DialogAddPartComponent, {
      data: this.expandedPart(),
      width: '500px',
    });

    dialogRef.afterClosed().subscribe((product) => {
      if (!product) return;

      // Create new Part
      let newPart = <PartModel>{};
      newPart.product = product;
      if (this.expandedPart()?.propertyTemplates) {
        newPart.properties = structuredClone(this.expandedPart()?.propertyTemplates!);
      }
      this.editService.currentPartId++;
      newPart.id = this.editService.currentPartId;

      // Add new Part to PartLink
      if (this.expandedPart()?.isCollection)
        this.expandedPart()?.parts?.push(newPart);
      else {
        if (this.expandedPart()?.parts?.length)
          this.expandedPart.update(item => {
            item!.parts![0] = newPart
            return item;
          });
        else this.expandedPart.update(item => {
          item!.parts?.push(newPart);
          return item;
        })
      }

      this.onSelectPartElement(newPart);
    });
  }

  removePart() {
    if (!this.expandedPart()) return;

    if (this.expandedPart()?.isCollection) {
      if (!this.selectedPart() || !this.expandedPart()?.parts) return;

      this.expandedPart.update(item => {
        item!.parts = item?.parts?.filter(
          (part) => part.id !== this.selectedPart()?.id
        );
        return item;
      })
    } else {
      this.expandedPart.update(item => {
        item!.parts = [] as PartModel[];
        return item;
      })
    }

    this.selectedPart.set(undefined);
    const url = this.getBaseUrl() + this.expandedPart()?.name + '/0';
    this.router.navigate([url]);
  }

  private getBaseUrl(): string {
    const url = this.router.url;
    const index = url.lastIndexOf('parts');
    let newUrl = url.substring(0, index);
    newUrl += 'parts/';
    return newUrl;
  }
}

