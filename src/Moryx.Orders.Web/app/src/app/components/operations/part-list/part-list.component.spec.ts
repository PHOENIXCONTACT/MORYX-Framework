/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartListComponent } from './part-list.component';

describe('PartListComponent', () => {
  let component: PartListComponent;
  let fixture: ComponentFixture<PartListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [PartListComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

