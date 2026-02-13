/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DefaultDetailsView } from './default-details-view';

describe('DefaultDetailsView', () => {
  let component: DefaultDetailsView;
  let fixture: ComponentFixture<DefaultDetailsView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DefaultDetailsView]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DefaultDetailsView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
