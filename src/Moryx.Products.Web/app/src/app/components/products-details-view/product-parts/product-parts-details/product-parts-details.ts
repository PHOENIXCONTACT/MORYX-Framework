/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, OnInit } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { PartConnector, PartModel } from '../../../../api/models';
import { EditProductsService } from '../../../../services/edit-products.service';

import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';

@Component({
  selector: 'app-product-parts-details',
  templateUrl: './product-parts-details.html',
  styleUrls: ['./product-parts-details.scss'],
  imports: [
    NavigableEntryEditor,
    TranslateModule
  ],
  standalone: true
})
export class ProductPartsDetailsComponent implements OnInit {
  partConnector = input.required<PartConnector>();
  productPart = input.required<PartModel>();

  TranslationConstants = TranslationConstants;

  constructor(
    public editService: EditProductsService,
    public translate: TranslateService
  ) {
  }

  ngOnInit(): void {
  }
}

