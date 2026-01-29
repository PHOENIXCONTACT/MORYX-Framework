/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersContainer } from './orders-container';

describe('OrdersContainer', () => {
  let component: OrdersContainer;
  let fixture: ComponentFixture<OrdersContainer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [OrdersContainer]
})
    .compileComponents();

    fixture = TestBed.createComponent(OrdersContainer);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

