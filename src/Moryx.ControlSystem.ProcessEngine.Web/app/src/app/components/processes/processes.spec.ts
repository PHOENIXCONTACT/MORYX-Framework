/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Processes } from './processes';

describe('Processes', () => {
  let component: Processes;
  let fixture: ComponentFixture<Processes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [Processes]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Processes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

