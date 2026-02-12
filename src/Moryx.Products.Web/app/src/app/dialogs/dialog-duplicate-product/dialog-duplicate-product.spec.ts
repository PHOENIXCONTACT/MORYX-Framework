/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogDuplicateProductComponent } from './dialog-duplicate-product';

describe('DialogDuplicateProductComponent', () => {
  let component: DialogDuplicateProductComponent;
  let fixture: ComponentFixture<DialogDuplicateProductComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogDuplicateProductComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogDuplicateProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

