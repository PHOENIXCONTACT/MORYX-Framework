/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Editor } from './editor';

describe('Editor', () => {
  let component: Editor;
  let fixture: ComponentFixture<Editor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [Editor]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Editor);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

