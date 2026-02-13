/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcessHolders } from './process-holders';

describe('ProcessHolders', () => {
  let component: ProcessHolders;
  let fixture: ComponentFixture<ProcessHolders>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProcessHolders]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProcessHolders);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

