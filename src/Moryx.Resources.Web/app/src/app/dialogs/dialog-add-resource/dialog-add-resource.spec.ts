/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAddResource } from './dialog-add-resource';

describe('DialogAddResource', () => {
  let component: DialogAddResource;
  let fixture: ComponentFixture<DialogAddResource>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogAddResource]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAddResource);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
