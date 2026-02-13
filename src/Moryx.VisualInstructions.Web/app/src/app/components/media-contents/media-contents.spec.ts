/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MediaContents } from './media-contents';

describe('MediaContents', () => {
  let component: MediaContents;
  let fixture: ComponentFixture<MediaContents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [MediaContents],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MediaContents);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

