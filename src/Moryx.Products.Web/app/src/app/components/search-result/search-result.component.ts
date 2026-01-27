/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, OnInit, signal } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { ProductModel, RevisionFilter, Selector } from 'src/app/api/models';
import { ProductManagementService } from 'src/app/api/services';
import { EditProductsService } from 'src/app/services/edit-products.service';

@Component({
  selector: 'app-search-result',
  templateUrl: './search-result.component.html',
  styleUrls: ['./search-result.component.scss'],
  standalone: true,
  imports: [
    MatListModule,
    EmptyState
  ]
})
export class SearchResultComponent implements OnInit {
  searchResults = signal<ProductModel[]>([]);
  searchString = signal('');

  constructor(
    public editService: EditProductsService,
    private managementService: ProductManagementService,
    private activatedRoute: ActivatedRoute
  ) {
  }

  ngOnInit(): void {
    this.activatedRoute.queryParamMap.subscribe((queryParam) => {
      this.onQueryParam(queryParam);
    });
  }

  getHref(productId: number | undefined): string {
    if (productId) {
      return '/Products/details/' + productId;
    }
    return '';
  }

  async onQueryParam(queryParam: ParamMap) {
    const searchString = queryParam.get('q');
    if (searchString) {
      this.searchString.update(_ => `*${searchString}*`);
    }
    const body = {
      includeDeleted: false,
      identifier: this.searchString(),
      revisionFilter: RevisionFilter[RevisionFilter.All],
      selector: Selector[Selector.Direct],
    };
    const result = await this.managementService
      .getTypes({body: body})
      .toAsync();
    this.searchResults.update(_ => result);
  }
}

