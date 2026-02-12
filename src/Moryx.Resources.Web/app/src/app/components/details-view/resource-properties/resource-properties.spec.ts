/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceProperties } from './resource-properties';

describe('ResourceProperties', () => {
  let component: ResourceProperties;
  let fixture: ComponentFixture<ResourceProperties>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ResourceProperties]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceProperties);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
