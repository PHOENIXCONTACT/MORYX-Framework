/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportDialog } from './report-dialog';

describe('ReportDialog', () => {
  let component: ReportDialog;
  let fixture: ComponentFixture<ReportDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ReportDialog]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

