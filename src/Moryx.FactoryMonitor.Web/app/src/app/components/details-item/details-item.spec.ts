/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsItem } from './details-item';

describe('DetailsItem', () => {
  let component: DetailsItem;
  let fixture: ComponentFixture<DetailsItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [DetailsItem]
})
    .compileComponents();

    fixture = TestBed.createComponent(DetailsItem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

