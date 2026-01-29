/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationDialog } from './configuration-dialog';

describe('ConfigurationDialog', () => {
  let component: ConfigurationDialog;
  let fixture: ComponentFixture<ConfigurationDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ConfigurationDialog]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurationDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

