/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { CellSettingsService } from './cell-settings.service';

describe('CellSettingsService', () => {
  let service: CellSettingsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CellSettingsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

