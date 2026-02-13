/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Assignment } from './assignment';

describe('Assignment', () => {
  let component: Assignment;
  let fixture: ComponentFixture<Assignment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Assignment]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(Assignment);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

