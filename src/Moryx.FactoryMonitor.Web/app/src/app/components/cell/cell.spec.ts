/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Cell } from './cell';

describe('Cell', () => {
  let component: Cell;
  let fixture: ComponentFixture<Cell>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [Cell]
})
    .compileComponents();

    fixture = TestBed.createComponent(Cell);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

