/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InterruptDialog } from './interrupt-dialog';

describe('InterruptDialog', () => {
  let component: InterruptDialog;
  let fixture: ComponentFixture<InterruptDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [InterruptDialog]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InterruptDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

