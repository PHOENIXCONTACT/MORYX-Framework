/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NodeProperties } from './node-properties';

describe('NodeProperties', () => {
  let component: NodeProperties;
  let fixture: ComponentFixture<NodeProperties>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [NodeProperties]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NodeProperties);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

