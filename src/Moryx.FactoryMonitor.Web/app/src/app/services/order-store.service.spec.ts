/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { OrderStoreService } from './order-store.service';

describe('OrderStoreServicesService', () => {
  let service: OrderStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrderStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

