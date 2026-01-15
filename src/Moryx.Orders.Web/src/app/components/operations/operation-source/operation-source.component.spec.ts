/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationSourceComponent } from './operation-source.component';

describe('OperationSourceComponent', () => {
  let component: OperationSourceComponent;
  let fixture: ComponentFixture<OperationSourceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OperationSourceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OperationSourceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

