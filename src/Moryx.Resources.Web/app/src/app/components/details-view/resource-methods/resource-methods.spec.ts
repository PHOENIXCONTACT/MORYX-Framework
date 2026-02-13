/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceMethods } from './resource-methods';

describe('ResourceMethods', () => {
  let component: ResourceMethods;
  let fixture: ComponentFixture<ResourceMethods>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ResourceMethods]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceMethods);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
