/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { AppStoreService } from './app-store.service';

describe('AppStoreService', () => {
  let service: AppStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AppStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

