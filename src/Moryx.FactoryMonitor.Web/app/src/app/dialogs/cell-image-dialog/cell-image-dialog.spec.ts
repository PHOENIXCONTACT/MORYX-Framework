/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CellImageDialog } from './cell-image-dialog';

describe('CellImageDialog', () => {
  let component: CellImageDialog;
  let fixture: ComponentFixture<CellImageDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [CellImageDialog]
})
    .compileComponents();

    fixture = TestBed.createComponent(CellImageDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

