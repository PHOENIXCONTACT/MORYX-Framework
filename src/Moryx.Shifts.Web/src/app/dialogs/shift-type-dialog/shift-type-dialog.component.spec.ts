/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShiftTypeDialogComponent } from './shift-type-dialog.component';

describe('ShiftTypeDialogComponent', () => {
  let component: ShiftTypeDialogComponent;
  let fixture: ComponentFixture<ShiftTypeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShiftTypeDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ShiftTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

