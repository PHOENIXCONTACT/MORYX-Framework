/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsDetailsView } from './products-details-view';

describe('ProductsDetailsViewComponent', () => {
  let component: ProductsDetailsView;
  let fixture: ComponentFixture<ProductsDetailsView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductsDetailsView]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductsDetailsView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

