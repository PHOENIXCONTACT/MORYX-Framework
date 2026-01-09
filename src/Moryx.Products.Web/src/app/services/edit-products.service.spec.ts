import { TestBed } from '@angular/core/testing';

import { EditProductsService } from './edit-products.service';

describe('EditProductsService', () => {
  let service: EditProductsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditProductsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
