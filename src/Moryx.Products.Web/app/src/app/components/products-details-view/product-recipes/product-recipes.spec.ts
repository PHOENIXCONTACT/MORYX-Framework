/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductRecipes } from './product-recipes';

describe('ProductRecipesComponent', () => {
  let component: ProductRecipes;
  let fixture: ComponentFixture<ProductRecipes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductRecipes]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductRecipes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

