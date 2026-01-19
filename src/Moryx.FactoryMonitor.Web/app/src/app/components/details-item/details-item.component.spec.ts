/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsItemComponent } from './details-item.component';

describe('DetailsItemComponent', () => {
  let component: DetailsItemComponent;
  let fixture: ComponentFixture<DetailsItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [DetailsItemComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(DetailsItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

