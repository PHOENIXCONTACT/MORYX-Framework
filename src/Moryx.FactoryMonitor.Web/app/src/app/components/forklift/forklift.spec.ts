/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Forklift } from './forklift';

describe('Forklift', () => {
  let component: Forklift;
  let fixture: ComponentFixture<Forklift>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [Forklift]
})
    .compileComponents();

    fixture = TestBed.createComponent(Forklift);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

