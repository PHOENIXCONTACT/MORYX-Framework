/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogShowRevisionsComponent } from './dialog-show-revisions';

describe('DialogShowRevisionsComponent', () => {
  let component: DialogShowRevisionsComponent;
  let fixture: ComponentFixture<DialogShowRevisionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogShowRevisionsComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogShowRevisionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

