import { Component, effect, input, Input, OnInit, untracked } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { PartConnector, PartModel } from '../../../../api/models';
import { EditProductsService } from '../../../../services/edit-products.service';
import { CommonModule } from '@angular/common';
import { NavigableEntryEditorComponent } from '@moryx/ngx-web-framework';
import { RouterLink } from '@angular/router';
import { MatIcon } from '@angular/material/icon';

@Component({
    selector: 'app-product-parts-details',
    templateUrl: './product-parts-details.component.html',
    styleUrls: ['./product-parts-details.component.scss'],
    imports: [
      CommonModule,
      NavigableEntryEditorComponent,
      TranslateModule,
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

  ngOnInit(): void {}
}
