/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsHeaderComponent } from './details-header.component';

describe('DetailsHeaderComponent', () => {
  let component: DetailsHeaderComponent;
  let fixture: ComponentFixture<DetailsHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DetailsHeaderComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailsHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

