/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { FormControlService } from './form-control-service.service';

describe('FormControlService', () => {
  let service: FormControlService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FormControlService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

