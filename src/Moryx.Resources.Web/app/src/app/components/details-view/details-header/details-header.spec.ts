/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsHeader } from './details-header';

describe('DetailsHeader', () => {
  let component: DetailsHeader;
  let fixture: ComponentFixture<DetailsHeader>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DetailsHeader]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailsHeader);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
