/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CellDetails } from './cell-details';

describe('CellDetails', () => {
  let component: CellDetails;
  let fixture: ComponentFixture<CellDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [CellDetails]
})
    .compileComponents();

    fixture = TestBed.createComponent(CellDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

