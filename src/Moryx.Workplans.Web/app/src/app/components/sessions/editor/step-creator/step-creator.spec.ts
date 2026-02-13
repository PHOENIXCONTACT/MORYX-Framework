/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepCreator } from './step-creator';

describe('StepCreator', () => {
  let component: StepCreator;
  let fixture: ComponentFixture<StepCreator>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [StepCreator]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepCreator);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

