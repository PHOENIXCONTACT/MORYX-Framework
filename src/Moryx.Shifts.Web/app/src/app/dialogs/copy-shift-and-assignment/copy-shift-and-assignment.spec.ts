/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CopyShiftAndAssignment } from './copy-shift-and-assignment';

describe('CopyShiftAndAssignment', () => {
  let component: CopyShiftAndAssignment;
  let fixture: ComponentFixture<CopyShiftAndAssignment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CopyShiftAndAssignment]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CopyShiftAndAssignment);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

