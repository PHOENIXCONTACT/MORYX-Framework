/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { FactorySelectionService } from './factory-selection.service';

describe('FactorySelectionService', () => {
  let service: FactorySelectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FactorySelectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

