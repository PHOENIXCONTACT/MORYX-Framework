/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { HttpErrorResponse } from '@angular/common/http';
import { Component, input, OnInit, signal } from '@angular/core';
import { MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ProductPartModel } from '../../../api/models';
import { OrderManagementService } from 'src/app/api/services';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { OperationViewModel } from 'src/app/models/operation-view-model';

@Component({
    selector: 'app-part-list',
    templateUrl: './part-list.component.html',
    styleUrls: ['./part-list.component.scss'],
    standalone: true,
    imports: [
    TranslateModule
],
})
export class PartListComponent implements OnInit {
  guid = input.required<string>();
  isLoading = signal(false);
  parts = signal<ProductPartModel[]>([]);
  TranslationConstants = TranslationConstants;

  constructor(
    private orderManagementService: OrderManagementService,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService
  ) {}

  ngOnInit(): void {
    this.isLoading.set(true);
    this.orderManagementService.getProductParts({ guid: this.guid() }).subscribe({
      next: value => {
        this.parts.set(value);
        this.isLoading.set(false);
      },
      error: async (e: HttpErrorResponse) => {
        await this.moryxSnackbar.handleError(e);
        this.isLoading.set(false);
      },
    });

  }
}

