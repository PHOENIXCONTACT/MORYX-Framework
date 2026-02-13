/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsImporter } from './products-importer';

describe('ProductsImporterComponent', () => {
  let component: ProductsImporter;
  let fixture: ComponentFixture<ProductsImporter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductsImporter]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductsImporter);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

