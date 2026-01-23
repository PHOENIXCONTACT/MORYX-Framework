/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { TestBed } from '@angular/core/testing';

import { EditMenuService } from './edit-menu.service';

describe('EditMenuService', () => {
  let service: EditMenuService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditMenuService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

