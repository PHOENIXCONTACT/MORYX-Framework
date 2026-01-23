/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { CacheProductsService } from './cache-products.service';

describe('CacheProductsService', () => {
  let service: CacheProductsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CacheProductsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

