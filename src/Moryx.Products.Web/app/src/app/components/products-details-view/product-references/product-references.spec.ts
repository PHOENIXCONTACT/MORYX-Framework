/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductReferencesComponent } from './product-references';

describe('ProductReferencesComponent', () => {
  let component: ProductReferencesComponent;
  let fixture: ComponentFixture<ProductReferencesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductReferencesComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductReferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

