/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { FactoryStateStreamService } from './factory-state-stream.service';

describe('FactoryStateStreamService', () => {
  let service: FactoryStateStreamService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FactoryStateStreamService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

