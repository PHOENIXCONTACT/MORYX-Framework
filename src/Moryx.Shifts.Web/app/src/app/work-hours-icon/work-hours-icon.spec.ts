/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkHoursIcon } from './work-hours-icon';

describe('WorkHoursIcon', () => {
  let component: WorkHoursIcon;
  let fixture: ComponentFixture<WorkHoursIcon>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkHoursIcon]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WorkHoursIcon);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

