/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationSource } from './operation-source';

describe('OperationSource', () => {
  let component: OperationSource;
  let fixture: ComponentFixture<OperationSource>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OperationSource ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OperationSource);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

