/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogCreateRecipeComponent } from './dialog-create-recipe';

describe('DialogCreateRecipeComponent', () => {
  let component: DialogCreateRecipeComponent;
  let fixture: ComponentFixture<DialogCreateRecipeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogCreateRecipeComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogCreateRecipeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

