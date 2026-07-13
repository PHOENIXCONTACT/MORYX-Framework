/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, inject, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { ProductModel, RevisionFilter, Selector } from '@api/models';
import { ProductManagementService } from '@api/services';
import { EditProductsService } from '@app/services/edit-products.service';

@Component({
  selector: 'app-search-result',
  templateUrl: './search-result.html',
  styleUrls: ['./search-result.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatListModule,
    EmptyState
  ]
})
export class SearchResult implements OnInit {
  private editProductsService = inject(EditProductsService);
  private productManagementService = inject(ProductManagementService);
  private activatedRoute = inject(ActivatedRoute);

  protected searchResults = signal<ProductModel[]>([]);
  protected searchString = signal('');

  ngOnInit(): void {
    this.activatedRoute.queryParamMap.subscribe((queryParam) => {
      this.onQueryParam(queryParam);
    });
  }

  protected getHref(productId: number | undefined): string {
    if (productId) {
      return '/Products/details/' + productId;
    }
    return '';
  }

  private async onQueryParam(queryParam: ParamMap) {
    const searchString = queryParam.get('q');
    if (searchString) {
      this.searchString.set(`*${searchString}*`);
    }
    const body = {
      includeDeleted: false,
      identifier: this.searchString(),
      revisionFilter: RevisionFilter[RevisionFilter.All],
      selector: Selector[Selector.Direct],
    };
    const result = await this.productManagementService
      .getTypes({body: body});
    this.searchResults.set(result);
  }

  protected createProductNameWithIdentity(product: ProductModel | undefined): string {
    return this.editProductsService.createProductNameWithIdentity(product);
  }
}

