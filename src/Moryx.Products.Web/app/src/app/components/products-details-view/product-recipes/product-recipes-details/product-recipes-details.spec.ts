/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductRecipesDetails } from './product-recipes-details';

describe('ProductRecipesDetailsComponent', () => {
  let component: ProductRecipesDetails;
  let fixture: ComponentFixture<ProductRecipesDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductRecipesDetails]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductRecipesDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

