/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { EditResourceService } from './edit-resource.service';

describe('EditResourceService', () => {
  let service: EditResourceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditResourceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

