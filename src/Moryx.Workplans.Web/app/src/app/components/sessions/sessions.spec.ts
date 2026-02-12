/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Sessions } from './sessions';

describe('Sessions', () => {
  let component: Sessions;
  let fixture: ComponentFixture<Sessions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [Sessions]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(Sessions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

