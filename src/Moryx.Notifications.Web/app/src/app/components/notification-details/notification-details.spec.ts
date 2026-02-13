/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationDetails } from './notification-details';

describe('NotificationDetails', () => {
  let component: NotificationDetails;
  let fixture: ComponentFixture<NotificationDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [NotificationDetails]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NotificationDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

