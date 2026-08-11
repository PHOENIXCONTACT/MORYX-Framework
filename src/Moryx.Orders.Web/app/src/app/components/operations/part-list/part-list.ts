/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, input, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { ProductPartModel } from '@api/models';
import { OrderManagementService } from '@api/services';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';

@Component({
  selector: 'app-part-list',
  templateUrl: './part-list.html',
  styleUrls: ['./part-list.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    TranslatePipe
  ]
})
export class PartList implements OnInit {
  readonly guid = input.required<string>();
  protected isLoading = signal(false);
  protected parts = signal<ProductPartModel[]>([]);
  protected TranslationConstants = TranslationConstants;

  private orderManagementService = inject(OrderManagementService);
  private snackbarService = inject(SnackbarService);

  ngOnInit(): void {
    this.isLoading.set(true);
    this.orderManagementService.getProductParts({guid: this.guid()}).then(value => {
      this.parts.set(value);
      this.isLoading.set(false);
    }).catch(async (e: HttpErrorResponse) => {
      await this.snackbarService.handleError(e);
      this.isLoading.set(false);
    });
  }
}

