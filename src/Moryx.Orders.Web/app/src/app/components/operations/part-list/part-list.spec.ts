/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartList } from './part-list';

describe('PartList', () => {
  let component: PartList;
  let fixture: ComponentFixture<PartList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [PartList]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

