/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogRemoveProductComponent } from './dialog-remove-product.component';

describe('DialogRemoveProductComponent', () => {
  let component: DialogRemoveProductComponent;
  let fixture: ComponentFixture<DialogRemoveProductComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogRemoveProductComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogRemoveProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

