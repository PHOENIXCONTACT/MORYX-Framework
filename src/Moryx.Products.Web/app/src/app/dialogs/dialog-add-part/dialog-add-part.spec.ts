/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAddPartComponent } from './dialog-add-part';

describe('DialogAddPartComponent', () => {
  let component: DialogAddPartComponent;
  let fixture: ComponentFixture<DialogAddPartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogAddPartComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAddPartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

