/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductParts } from './product-parts';

describe('ProductPartsComponent', () => {
  let component: ProductParts;
  let fixture: ComponentFixture<ProductParts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductParts],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductParts);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

