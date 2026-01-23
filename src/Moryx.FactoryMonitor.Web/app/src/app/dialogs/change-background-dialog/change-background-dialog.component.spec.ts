/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeBackgroundDialogComponent } from './change-background-dialog.component';

describe('ChangeBackgroundDialogComponent', () => {
  let component: ChangeBackgroundDialogComponent;
  let fixture: ComponentFixture<ChangeBackgroundDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [ChangeBackgroundDialogComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(ChangeBackgroundDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

