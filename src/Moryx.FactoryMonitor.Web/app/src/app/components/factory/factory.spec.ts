/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Factory } from './factory';

describe('Factory', () => {
  let component: Factory;
  let fixture: ComponentFixture<Factory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: []
})
    .compileComponents();

    fixture = TestBed.createComponent(Factory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

