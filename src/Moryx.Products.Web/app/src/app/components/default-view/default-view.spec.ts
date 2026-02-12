/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DefaultView } from './default-view';

describe('DefaultViewComponent', () => {
  let component: DefaultView;
  let fixture: ComponentFixture<DefaultView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DefaultView],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DefaultView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

