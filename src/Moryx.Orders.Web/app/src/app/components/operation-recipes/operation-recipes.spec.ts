/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationRecipes } from './operation-recipes';

describe('OperationRecipes', () => {
  let component: OperationRecipes;
  let fixture: ComponentFixture<OperationRecipes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [OperationRecipes]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OperationRecipes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

