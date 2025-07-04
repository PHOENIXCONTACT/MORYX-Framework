import { Component, signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ProductModel } from '../../../api/models';
import { EditProductsService } from '../../../services/edit-products.service';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { EmptyStateComponent } from '@moryx/ngx-web-framework';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-product-references',
    templateUrl: './product-references.component.html',
    styleUrls: ['./product-references.component.scss'],
    standalone: true,
    imports: [
      CommonModule,
      MatTableModule,
      TranslateModule,
      EmptyStateComponent,
      MatProgressSpinnerModule,
      MatCardModule
    ]
})
export class ProductReferencesComponent {

  references = signal<ProductModel[]>([]);
  isLoading = signal(false);
  TranslationConstants = TranslationConstants;

  constructor(
    editService: EditProductsService,
    public translate: TranslateService,
    private router: Router
  ) {
    this.isLoading.update(_=> true);
    editService.references.subscribe((references) => {
      this.references.update(_=> references ?? []);
      this.isLoading.update(_=> false);
    });
  }

  referenceClicked(reference: ProductModel) {
    this.router.navigate(['/details',reference.id,'properties'])
  }
}
