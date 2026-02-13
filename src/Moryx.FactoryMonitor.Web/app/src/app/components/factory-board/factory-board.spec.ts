/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FactoryBoard } from './factory-board';

describe('FactoryBoard', () => {
  let component: FactoryBoard;
  let fixture: ComponentFixture<FactoryBoard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FactoryBoard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FactoryBoard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

