/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DefaultViewComponent } from './default-view.component';

describe('DefaultViewComponent', () => {
  let component: DefaultViewComponent;
  let fixture: ComponentFixture<DefaultViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DefaultViewComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DefaultViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

