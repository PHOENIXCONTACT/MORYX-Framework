import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { defer } from 'rxjs';
import { first} from 'rxjs/operators';
import { EditProductsService } from '../../services/edit-products.service';
import { ProductModel } from '../../api/models';

export const ProductsDetailsViewResolver: ResolveFn<ProductModel> = (route: ActivatedRouteSnapshot) => {
  const editService = inject(EditProductsService);
  const id = Number(route.paramMap.get('id'));

  return defer(() => {
    editService.loadProductById(id);
    return editService.currentProduct.pipe(
      first((product): product is ProductModel => product !== undefined && product.id === id)
    );
  });
};
