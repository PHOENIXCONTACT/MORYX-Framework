/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { InstructionService } from './instruction.service';

describe('IntructionsService', () => {
  let service: InstructionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(InstructionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

