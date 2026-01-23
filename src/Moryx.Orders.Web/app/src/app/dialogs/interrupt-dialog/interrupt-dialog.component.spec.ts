/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportDialogComponent } from './interrupt-dialog.component';

describe('InterruptDialogComponent', () => {
  let component: ReportDialogComponent;
  let fixture: ComponentFixture<ReportDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ReportDialogComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

