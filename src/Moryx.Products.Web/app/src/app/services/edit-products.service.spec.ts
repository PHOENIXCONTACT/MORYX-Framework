/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

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

