/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductReferences } from './product-references';

describe('ProductReferencesComponent', () => {
  let component: ProductReferences;
  let fixture: ComponentFixture<ProductReferences>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductReferences],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductReferences);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

