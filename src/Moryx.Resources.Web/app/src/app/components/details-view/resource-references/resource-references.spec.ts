/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceReferences } from './resource-references';

describe('ResourceReferences', () => {
  let component: ResourceReferences;
  let fixture: ComponentFixture<ResourceReferences>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ResourceReferences]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceReferences);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
