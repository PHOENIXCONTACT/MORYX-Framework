/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerInstructions } from './worker-instructions';

describe('WorkerInstructions', () => {
  let component: WorkerInstructions;
  let fixture: ComponentFixture<WorkerInstructions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [WorkerInstructions]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerInstructions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

