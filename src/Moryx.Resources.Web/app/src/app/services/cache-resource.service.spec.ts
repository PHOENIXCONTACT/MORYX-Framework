/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { CacheResourceService } from './cache-resource.service';

describe('CacheResourceService', () => {
  let service: CacheResourceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CacheResourceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

