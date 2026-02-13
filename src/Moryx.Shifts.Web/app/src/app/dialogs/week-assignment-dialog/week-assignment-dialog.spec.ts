/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeekAssignmentDialog } from './week-assignment-dialog';

describe('WeekAssignmentDialog', () => {
  let component: WeekAssignmentDialog;
  let fixture: ComponentFixture<WeekAssignmentDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WeekAssignmentDialog]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WeekAssignmentDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

