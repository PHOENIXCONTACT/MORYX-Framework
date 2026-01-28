/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Toolbox } from './toolbox';

describe('Toolbox', () => {
  let component: Toolbox;
  let fixture: ComponentFixture<Toolbox>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [Toolbox]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Toolbox);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

