/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Operations } from './operations';

describe('Operations', () => {
  let component: Operations;
  let fixture: ComponentFixture<Operations>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [Operations]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Operations);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

