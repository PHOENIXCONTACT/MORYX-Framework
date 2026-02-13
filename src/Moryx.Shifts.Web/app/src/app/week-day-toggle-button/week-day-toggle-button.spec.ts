/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeekDayToggleButton } from './week-day-toggle-button';

describe('WeekDayToggleButton', () => {
  let component: WeekDayToggleButton;
  let fixture: ComponentFixture<WeekDayToggleButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WeekDayToggleButton]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WeekDayToggleButton);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

