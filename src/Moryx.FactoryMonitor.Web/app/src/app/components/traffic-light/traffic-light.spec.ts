/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrafficLight } from './traffic-light';

describe('TrafficLight', () => {
  let component: TrafficLight;
  let fixture: ComponentFixture<TrafficLight>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [TrafficLight]
})
    .compileComponents();

    fixture = TestBed.createComponent(TrafficLight);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

