/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShiftInstanceDialog } from './shift-instance-dialog';

describe('ShiftInstanceDialog', () => {
  let component: ShiftInstanceDialog;
  let fixture: ComponentFixture<ShiftInstanceDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShiftInstanceDialog]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ShiftInstanceDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

