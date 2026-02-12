/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductProperties } from './product-properties';

describe('ProductPropertiesComponent', () => {
  let component: ProductProperties;
  let fixture: ComponentFixture<ProductProperties>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductProperties],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductProperties);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

