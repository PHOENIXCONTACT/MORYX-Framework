/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductRecipesDetailsHeader } from './product-recipes-details-header';

describe('ProductRecipesDetailsHeaderComponent', () => {
  let component: ProductRecipesDetailsHeader;
  let fixture: ComponentFixture<ProductRecipesDetailsHeader>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductRecipesDetailsHeader],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductRecipesDetailsHeader);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

