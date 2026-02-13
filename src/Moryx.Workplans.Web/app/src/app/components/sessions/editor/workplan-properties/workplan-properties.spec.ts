/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkplanProperties } from './workplan-properties';

describe('WorkplanProperties', () => {
  let component: WorkplanProperties;
  let fixture: ComponentFixture<WorkplanProperties>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [WorkplanProperties]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkplanProperties);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

