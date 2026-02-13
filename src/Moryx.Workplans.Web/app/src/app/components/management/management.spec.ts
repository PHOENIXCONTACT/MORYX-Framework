/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Management } from './management';

describe('Management', () => {
  let component: Management;
  let fixture: ComponentFixture<Management>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [Management]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Management);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

