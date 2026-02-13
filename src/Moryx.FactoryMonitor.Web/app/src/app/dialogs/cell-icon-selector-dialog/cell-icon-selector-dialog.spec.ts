/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CellIconUploaderDialog } from './cell-icon-selector-dialog';

describe('CellIconUploaderDialog', () => {
  let component: CellIconUploaderDialog;
  let fixture: ComponentFixture<CellIconUploaderDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [CellIconUploaderDialog]
})
    .compileComponents();

    fixture = TestBed.createComponent(CellIconUploaderDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

