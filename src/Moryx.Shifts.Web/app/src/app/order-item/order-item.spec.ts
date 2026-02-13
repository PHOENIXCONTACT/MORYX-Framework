/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderItem } from './order-item';

describe('OrderItem', () => {
  let component: OrderItem;
  let fixture: ComponentFixture<OrderItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderItem]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(OrderItem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

