/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsConfigurationDialog } from './details-configuration-dialog';

describe('DetailsConfigurationDialog', () => {
  let component: DetailsConfigurationDialog;
  let fixture: ComponentFixture<DetailsConfigurationDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [DetailsConfigurationDialog]
})
    .compileComponents();

    fixture = TestBed.createComponent(DetailsConfigurationDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

