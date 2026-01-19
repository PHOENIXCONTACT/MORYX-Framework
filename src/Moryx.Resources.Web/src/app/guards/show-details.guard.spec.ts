/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { ShowDetailsGuard } from './show-details.guard';

describe('ShowDetailsGuard', () => {
  let guard: ShowDetailsGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ShowDetailsGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});

