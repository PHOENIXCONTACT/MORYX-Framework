/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeBackgroundDialog } from './change-background-dialog';

describe('ChangeBackgroundDialog', () => {
  let component: ChangeBackgroundDialog;
  let fixture: ComponentFixture<ChangeBackgroundDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [ChangeBackgroundDialog]
})
    .compileComponents();

    fixture = TestBed.createComponent(ChangeBackgroundDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

