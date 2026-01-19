/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAddResourceComponent } from './dialog-add-resource.component';

describe('DialogAddResourceComponent', () => {
  let component: DialogAddResourceComponent;
  let fixture: ComponentFixture<DialogAddResourceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogAddResourceComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAddResourceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

