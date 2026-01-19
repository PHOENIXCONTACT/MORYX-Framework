/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { ImporterGuard } from './importer.guard';

describe('ImporterGuard', () => {
  let guard: ImporterGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ImporterGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});

