/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationDialogComponent } from './configuration-dialog.component';

describe('ConfigurationDialogComponent', () => {
  let component: ConfigurationDialogComponent;
  let fixture: ComponentFixture<ConfigurationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ConfigurationDialogComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

