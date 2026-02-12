/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogMessageList } from './log-message-list';

describe('LogMessageList', () => {
  let component: LogMessageList;
  let fixture: ComponentFixture<LogMessageList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [LogMessageList],
    providers: [
        {}
    ]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LogMessageList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

