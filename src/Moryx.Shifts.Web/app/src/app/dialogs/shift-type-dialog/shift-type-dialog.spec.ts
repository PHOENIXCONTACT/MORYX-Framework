/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShiftTypeDialog } from './shift-type-dialog';

describe('ShiftTypeDialog', () => {
  let component: ShiftTypeDialog;
  let fixture: ComponentFixture<ShiftTypeDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShiftTypeDialog]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ShiftTypeDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

