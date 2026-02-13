/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VariantOverviewComponent } from './variant-overview.component';

describe('VariantOverviewComponent', () => {
  let component: VariantOverviewComponent;
  let fixture: ComponentFixture<VariantOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [VariantOverviewComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(VariantOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

